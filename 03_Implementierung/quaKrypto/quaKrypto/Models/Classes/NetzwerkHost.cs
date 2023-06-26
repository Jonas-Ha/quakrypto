using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace quaKrypto.Models.Classes
{
    public static class NetzwerkHost
    {
        private const byte LOBBYINFORMATION = 0x01;
        private const byte LOBBY_NICHT_MEHR_VERFUEGBAR = 0x02;
        private const byte ROLLENINFORMATION = 0x03;
        private const byte ROLLE_WAEHLEN = 0x04;
        private const byte ROLLE_FREIGEBEN = 0x05;
        private const byte UEBUNGSSZENARIO_STARTEN = 0x06;
        private const byte KONTROLLE_UEBERGEBEN = 0x07;
        private const byte ZUG_BEENDEN = 0x08;
        private const byte AUFZEICHNUNG_UPDATE = 0x09;
        private const byte UEBUNGSSZENARIO_ENDE = 0x10;

        private const int ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS = 1000;

        private const int UDP_LISTEN_PORT = 18523;
        private const int UDP_SEND_PORT = 18524;

        private static UdpClient? udpClient = null;
        private static TcpListener? tcpListener = null;

        private const int TCP_RECEIVE_BUFFER_SIZE = 131072;

        private static readonly Dictionary<RolleEnum, NetworkStream> rolleNetworkStreams = new();
        private static readonly List<NetworkStream> networkStreams = new();

        private static Rolle? aliceRolle, bobRolle, eveRolle;

        public static Rolle? AliceRolle { get { return aliceRolle; } set { aliceRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.AliceState = aliceRolle != null; } }
        public static Rolle? BobRolle { get { return bobRolle; } set { bobRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.BobState = bobRolle != null; } }
        public static Rolle? EveRolle { get { return eveRolle; } set { eveRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.EveState = eveRolle != null; } }

        private static UebungsszenarioNetzwerk? uebungsszenario;
        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }

        private static UebungsszenarioNetzwerkBeitrittInfo? uebungsszenarioNetzwerkBeitrittInfo;

        public static bool BeendenErlaubt { get; set; }

        public static void ResetNetzwerkHost()
        {
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            tcpListener?.Stop();
            tcpListener = null;
            rolleNetworkStreams.Clear();
            networkStreams.Clear();
            aliceRolle = null;
            bobRolle = null;
            eveRolle = null;
            uebungsszenario = null;
            uebungsszenarioNetzwerkBeitrittInfo = null;
        }


        #region UDP

        //Schnittstelle für LobbyScreenView im Konstruktor oder LobbyerstellenView am Ende
        public static async void BeginneZyklischesSendenVonLobbyinformation(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo, int portToSendTo = UDP_LISTEN_PORT)
        {
            if (udpClient != null) return;
            uebungsszenarioNetzwerkBeitrittInfo = netzwerkBeitrittInfo;
            ErstelleTCPLobby();
            udpClient = new UdpClient(UDP_SEND_PORT);
            using PeriodicTimer periodicTimer = new(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                if (udpClient == null) break;
                string netzwerkBeitrittInfoAsString = $"{uebungsszenarioNetzwerkBeitrittInfo.Lobbyname.Replace("\t", "")}\t{uebungsszenarioNetzwerkBeitrittInfo.Protokoll}\t{uebungsszenarioNetzwerkBeitrittInfo.Variante}\t{uebungsszenarioNetzwerkBeitrittInfo.Schwierigkeitsgrad}\t{uebungsszenarioNetzwerkBeitrittInfo.AliceState}\t{uebungsszenarioNetzwerkBeitrittInfo.BobState}\t{uebungsszenarioNetzwerkBeitrittInfo.EveState}\t{uebungsszenarioNetzwerkBeitrittInfo.StartPhase}\t{uebungsszenarioNetzwerkBeitrittInfo.EndPhase}\t{uebungsszenarioNetzwerkBeitrittInfo.HostPort}";
                byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(netzwerkBeitrittInfoAsString);
                byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
                nachrichtZumSenden[0] = LOBBYINFORMATION;
                Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
                try
                {
                    udpClient?.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", portToSendTo);
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Senden vom Host geworfen"); break; }
            }
            BeendeZyklischesSendenVonLobbyinformation();
        }
        public static void BeendeZyklischesSendenVonLobbyinformation()
        {
            if (udpClient == null) return;
            try
            {
                udpClient?.Send(new byte[] { LOBBY_NICHT_MEHR_VERFUEGBAR }, 1, "255.255.255.255", UDP_LISTEN_PORT);
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
            }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #region TCP
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht, RolleEnum? empfänger = null)
        {
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 4];
            nachrichtZumSenden[0] = commandIdentifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            for (int i = 0; i < 3; i++)
            {
                nachrichtZumSenden[^(1 + i)] = (byte)'\0';
            }
            try
            {
                if (empfänger == null || commandIdentifier.Equals(KONTROLLE_UEBERGEBEN))
                {
                    foreach (NetworkStream networkStream in networkStreams)
                    {
                        networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                    }
                    foreach (NetworkStream networkStream in rolleNetworkStreams.Values)
                    {
                        networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                    }
                }
                else
                {
                    NetworkStream networkStream = rolleNetworkStreams[(RolleEnum)empfänger];
                    networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                }
            }
            catch (Exception) { }
        }

        private static void ErstelleTCPLobby()
        {
            if (tcpListener != null) return;
            new Thread(() =>
            {
                try
                {
                    tcpListener = new(IPAddress.Any, 0);
                    tcpListener.Start();
                    if (uebungsszenarioNetzwerkBeitrittInfo != null && tcpListener.Server.LocalEndPoint != null)
                        uebungsszenarioNetzwerkBeitrittInfo.HostPort = ((IPEndPoint)tcpListener.Server.LocalEndPoint).Port;
                    while (true)
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();

                        NetworkStream networkStream = tcpClient.GetStream();
                        networkStreams.Add(networkStream);
                        StarteTCPListeningThread(networkStream);
                        Thread.Sleep(100); //Zum Teil wird sonst nicht richtig die RollenInformation gesendet
                        SendeRollenInformation();
                    }
                }
                catch (SocketException) { BeendeTCPLobby(); Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Verbindung Annehmen als Host geworfen"); }
            }).Start();
        }

        //Schnittstelle für LobbyScreenView
        public static void BeendeTCPLobby()
        {
            SendeNachrichtTCP(LOBBY_NICHT_MEHR_VERFUEGBAR, "");
            tcpListener?.Stop();
            tcpListener = null;
            foreach (NetworkStream networkStream in networkStreams)
            {
                networkStream.Close();
            }
            foreach (NetworkStream networkStream in rolleNetworkStreams.Values)
            {
                networkStream.Close();
            }
            BeendeZyklischesSendenVonLobbyinformation();
        }

        //Schnittstelle fürs Übungsszenario (Wenn du selber eine Rolle wählst)
        public static void SendeRollenInformation()
        {
            SendeNachrichtTCP(ROLLENINFORMATION, (AliceRolle == null ? "" : AliceRolle.Alias) + '\t' + (BobRolle == null ? "" : BobRolle?.Alias) + '\t' + (EveRolle == null ? "" : EveRolle?.Alias));
        }

        //Schnittstelle für LobbyScreenView
        public static void StarteUebungsszenario(RolleEnum startRolle, int seed)
        {
            SendeNachrichtTCP(UEBUNGSSZENARIO_STARTEN, $"{startRolle}\t{seed}");
            BeendeZyklischesSendenVonLobbyinformation();
        }

        //Schnittstelle fürs Übungsszenario
        public static void UebergebeKontrolle(RolleEnum nächsteRolle) => SendeNachrichtTCP(KONTROLLE_UEBERGEBEN, nächsteRolle.ToString(), nächsteRolle);

        //Schnittstelle fürs Übungsszenario
        public static void SendeAufzeichnungsUpdate(List<Handlungsschritt> neueHandlungsschritte, RolleEnum? empfänger = null)
        {
            //Eventuell Encoding vom Serializer benutzen oder anderweitig lösen
            XmlSerializer xmlSerializer = new(typeof(List<Handlungsschritt>));
            using StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, neueHandlungsschritte);
            SendeNachrichtTCP(AUFZEICHNUNG_UPDATE, stringWriter.ToString(), empfänger);
        }

        //Schnittstelle fürs Übungsszenario
        public static void BeendeUebungsszenario()
        {
            if (!BeendenErlaubt) return;
            SendeNachrichtTCP(UEBUNGSSZENARIO_ENDE, "");
            BeendeTCPLobby();
        }

        private static void StarteTCPListeningThread(NetworkStream networkStream)
        {
            new Thread(() =>
            {
                byte[] kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                while (true)
                {
                    try
                    {
                        networkStream.Read(kompletteNachrichtAlsBytes, 0, TCP_RECEIVE_BUFFER_SIZE);
                        string[] empfangeneGanzeNachrichten = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes).Split("\0\0\0");
                        foreach (string ganzeNachricht in empfangeneGanzeNachrichten)
                        {
                            if (ganzeNachricht == "") break;
                            byte commandIdentifier = (byte)ganzeNachricht[0];
                            string[] empfangeneNachrichtTeile = ganzeNachricht[1..].Split('\t');
                            for (int i = 0; i < empfangeneNachrichtTeile.Length; i++) empfangeneNachrichtTeile[i] = empfangeneNachrichtTeile[i].TrimEnd('\0');
                            switch (commandIdentifier)
                            {
                                case ROLLE_WAEHLEN:
                                    if (Enum.TryParse(empfangeneNachrichtTeile[0], out RolleEnum neueRolle))
                                    {
                                        switch (neueRolle)
                                        {
                                            case RolleEnum.Alice:
                                                aliceRolle = new Rolle(RolleEnum.Alice, empfangeneNachrichtTeile[1], "");
                                                uebungsszenario?.RolleHinzufuegen(aliceRolle, false);
                                                break;
                                            case RolleEnum.Bob:
                                                bobRolle = new Rolle(RolleEnum.Bob, empfangeneNachrichtTeile[1], "");
                                                uebungsszenario?.RolleHinzufuegen(bobRolle, false);
                                                break;
                                            case RolleEnum.Eve:
                                                eveRolle = new Rolle(RolleEnum.Eve, empfangeneNachrichtTeile[1], "");
                                                uebungsszenario?.RolleHinzufuegen(eveRolle, false);
                                                break;
                                        }
                                        networkStreams.Remove(networkStream);
                                        rolleNetworkStreams[neueRolle] = networkStream;
                                    }
                                    break;
                                case ROLLE_FREIGEBEN:
                                    if (Enum.TryParse(empfangeneNachrichtTeile[0], out RolleEnum alteRolle))
                                    {
                                        switch (alteRolle)
                                        {
                                            case RolleEnum.Alice:
                                                aliceRolle = null;
                                                uebungsszenario?.GebeRolleFrei(RolleEnum.Alice);
                                                break;
                                            case RolleEnum.Bob:
                                                bobRolle = null;
                                                uebungsszenario?.GebeRolleFrei(RolleEnum.Bob);
                                                break;
                                            case RolleEnum.Eve:
                                                eveRolle = null;
                                                uebungsszenario?.GebeRolleFrei(RolleEnum.Eve);
                                                break;
                                        }
                                        networkStreams.Add(networkStream);
                                        rolleNetworkStreams.Remove(alteRolle);
                                        uebungsszenario?.GebeRolleFrei(alteRolle);
                                    }
                                    break;
                                case ZUG_BEENDEN:
                                    List<Handlungsschritt> listeEmpfangenerHandlungsschritte = new();
                                    XmlSerializer xmlHandlungsschrittSerializer = new(typeof(List<Handlungsschritt>));
                                    using (StringReader stringReader = new StringReader(empfangeneNachrichtTeile[0]))
                                    {
                                        object? deserialisiertesObjekt = xmlHandlungsschrittSerializer.Deserialize(stringReader);
                                        if (deserialisiertesObjekt != null)
                                        {
                                            uebungsszenario?.ZugWurdeBeendet((List<Handlungsschritt>)deserialisiertesObjekt);
                                        }
                                    }
                                    break;
                                case UEBUNGSSZENARIO_ENDE:
                                    uebungsszenario?.Beenden();
                                    break;
                            }
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (Exception)
                    {
                        networkStream.Close();
                        if (rolleNetworkStreams.ContainsValue(networkStream))
                        {
                            RolleEnum rolle = rolleNetworkStreams.Where(n => n.Value == networkStream).First().Key;
                            switch (rolle)
                            {
                                case RolleEnum.Alice:
                                    aliceRolle = null;
                                    uebungsszenario?.GebeRolleFrei(RolleEnum.Alice);
                                    break;
                                case RolleEnum.Bob:
                                    bobRolle = null;
                                    uebungsszenario?.GebeRolleFrei(RolleEnum.Bob);
                                    break;
                                case RolleEnum.Eve:
                                    eveRolle = null;
                                    uebungsszenario?.GebeRolleFrei(RolleEnum.Eve);
                                    break;
                            }
                            rolleNetworkStreams.Remove(rolle);
                        }
                        networkStreams.Remove(networkStream);

                        Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen mit folgender Adresse geworfen: ");
                        break;
                    }
                }
            }).Start();
        }

        #endregion

    }
}
