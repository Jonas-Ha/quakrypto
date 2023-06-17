using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
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

        private static PeriodicTimer? periodicTimer;

        private const int UDP_PORT = 18523;
        private const int TCP_PORT = 32581;

        private static UdpClient? udpClient = null;
        private static TcpListener? tcpListener = null;

        private const int TCP_RECEIVE_BUFFER_SIZE = 8192;

        private static readonly Dictionary<RolleEnum, NetworkStream> rolleNetworkStreams = new();
        private static readonly List<NetworkStream> networkStreams = new();

        private static Rolle? aliceRolle, bobRolle, eveRolle;

        public static Rolle? AliceRolle { get { return aliceRolle; } set { aliceRolle = value; if(uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.AliceState = aliceRolle != null; } }
        public static Rolle? BobRolle { get { return bobRolle; } set { bobRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.BobState = bobRolle != null; } }
        public static Rolle? EveRolle { get { return eveRolle; } set { eveRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.EveState = eveRolle != null; } }

        private static UebungsszenarioNetzwerk? uebungsszenario;
        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }


        private static UebungsszenarioNetzwerkBeitrittInfo? uebungsszenarioNetzwerkBeitrittInfo;


        #region UDP

        //Schnittstelle für LobbyScreenView im Konstruktor oder LobbyerstellenView am Ende
        public static async void BeginneZyklischesSendenVonLobbyinformation(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo, int portToSendTo = UDP_PORT)
        {
            if (udpClient != null) return;
            uebungsszenarioNetzwerkBeitrittInfo = netzwerkBeitrittInfo;
            udpClient = new UdpClient(UDP_PORT);
            periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
            ErstelleTCPLobby();
            while (await periodicTimer.WaitForNextTickAsync())
            {
                string netzwerkBeitrittInfoAsString = uebungsszenarioNetzwerkBeitrittInfo.Lobbyname.Replace("\t", "") + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.Protokoll + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.Variante + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.Schwierigkeitsgrad.ToString() + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.AliceState.ToString() + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.BobState.ToString() + '\t'
                    + uebungsszenarioNetzwerkBeitrittInfo.EveState.ToString();

                byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(netzwerkBeitrittInfoAsString);
                byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
                nachrichtZumSenden[0] = LOBBYINFORMATION;
                Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
                try
                {
                    udpClient.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", portToSendTo);
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Senden vom Host geworfen"); break; }

            }

        }
        private static void BeendeZyklischesSendenVonLobbyinformation()
        {
            if (udpClient == null) return;
            periodicTimer?.Dispose();
            periodicTimer = null;
            udpClient.Send(new byte[] { LOBBY_NICHT_MEHR_VERFUEGBAR }, 1, "255.255.255.255", UDP_PORT);
            udpClient.Close();
            udpClient = null;
        }

        #endregion

        #region TCP
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht, RolleEnum? empfänger = null)
        {
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
            nachrichtZumSenden[0] = commandIdentifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            if (empfänger == null)
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

        private static void ErstelleTCPLobby()
        {
            if (tcpListener != null) return;
            new Thread(() =>
            {
                try
                {
                    tcpListener = new(IPAddress.Any, TCP_PORT);
                    tcpListener.Start();
                    while (true)
                    {
                        Trace.WriteLine("Starting To Accept");
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        Trace.WriteLine("Accepted Client");
                        NetworkStream networkStream = tcpClient.GetStream();
                        networkStreams.Add(networkStream);
                        //TODO: Client schicken 
                        StarteTCPListeningThread(networkStream);
                    }
                }
                catch (SocketException) { BeendeTCPLobby(); Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Verbindung Annehmen als Host geworfen"); }
            }).Start();
        }

        //Schnittstelle für LobbyScreenView
        public static void BeendeTCPLobby()
        {
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
            SendeNachrichtTCP(ROLLENINFORMATION, AliceRolle?.Alias + '\t' + BobRolle?.Alias + '\t' + EveRolle?.Alias);
        }

        //Schnittstelle für LobbyScreenView
        public static void StarteUebungsszenario()
        {
            SendeNachrichtTCP(UEBUNGSSZENARIO_STARTEN, "");
            BeendeZyklischesSendenVonLobbyinformation();
        }

        //Schnittstelle fürs Übungsszenario
        public static void UebergebeKontrolle(RolleEnum nächsteRolle)
        {
            SendeNachrichtTCP(KONTROLLE_UEBERGEBEN, "", nächsteRolle);
        }

        //Schnittstelle fürs Übungsszenario
        public static void SendeAufzeichnungsUpdate(List<Handlungsschritt> neueHandlungsschritte, RolleEnum? empfänger = null)
        {
            string serializedHandlungsschritte = new("");
            XmlSerializer xmlSerializer = new(typeof(Handlungsschritt));
            for (int i = 0; i < neueHandlungsschritte.Count; i++)
            {
                if (i != 0) serializedHandlungsschritte += '\t';
                using StringWriter stringWriter = new();
                xmlSerializer.Serialize(stringWriter, neueHandlungsschritte[i]);
                serializedHandlungsschritte += stringWriter.ToString();
            }
            SendeNachrichtTCP(AUFZEICHNUNG_UPDATE, serializedHandlungsschritte, empfänger);
        }

        //Schnittstelle fürs Übungsszenario
        public static void BeendeUebungsszenario()
        {
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
                        byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                        string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                        switch (commandIdentifier)
                        {
                            case ROLLE_WAEHLEN:
                                if (Enum.TryParse(empfangeneNachrichtTeile[0], out RolleEnum neueRolle))
                                {
                                    switch (neueRolle)
                                    {
                                        case RolleEnum.Alice:
                                            aliceRolle = new Rolle(RolleEnum.Alice, empfangeneNachrichtTeile[1]);
                                            uebungsszenario?.RolleHinzufuegen(aliceRolle);
                                            break;
                                        case RolleEnum.Bob:
                                            bobRolle = new Rolle(RolleEnum.Bob, empfangeneNachrichtTeile[1]);
                                            uebungsszenario?.RolleHinzufuegen(bobRolle);
                                            break;
                                        case RolleEnum.Eve:
                                            eveRolle = new Rolle(RolleEnum.Eve, empfangeneNachrichtTeile[1]);
                                            uebungsszenario?.RolleHinzufuegen(eveRolle);
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
                                            break;
                                        case RolleEnum.Bob:
                                            bobRolle = null;
                                            break;
                                        case RolleEnum.Eve:
                                            eveRolle = null;
                                            break;
                                    }
                                    networkStreams.Add(networkStream);
                                    rolleNetworkStreams.Remove(alteRolle);
                                    uebungsszenario?.GebeRolleFrei(alteRolle);
                                }
                                break;
                            case ZUG_BEENDEN:
                                List<Handlungsschritt> listeEmpfangenerHandlungsschritte = new();
                                XmlSerializer xmlSerializer = new(typeof(Handlungsschritt));
                                foreach (string handlungsschritt in empfangeneNachrichtTeile)
                                {
                                    using StringReader stringReader = new StringReader(handlungsschritt);
                                    object? deserialisiertesObjekt = xmlSerializer.Deserialize(stringReader);
                                    if (deserialisiertesObjekt != null)
                                    {
                                        listeEmpfangenerHandlungsschritte.Add((Handlungsschritt)deserialisiertesObjekt);
                                    }
                                }
                                uebungsszenario?.ZugWurdeBeendet(listeEmpfangenerHandlungsschritte);
                                break;
                            case UEBUNGSSZENARIO_ENDE:
                                uebungsszenario?.Beenden();
                                break;
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (SocketException) { networkStream.Close(); Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen mit folgender Adresse geworfen: "); break; }
                }
            }).Start();
        }

        #endregion

    }
}
