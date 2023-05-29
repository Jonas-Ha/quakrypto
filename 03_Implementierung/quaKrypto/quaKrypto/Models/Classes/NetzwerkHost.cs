using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;

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

        private static Dictionary<Rolle, NetworkStream> rolleNetworkStreams = new();
        private static List<NetworkStream> networkStreams = new();

        private static Rolle? aliceRolle, bobRolle, eveRolle;

        public static Rolle? AliceRolle { get { return aliceRolle; } }
        public static Rolle? BobRolle { get { return bobRolle; } }
        public static Rolle? EveRolle { get { return eveRolle; } }

        private static UebungsszenarioNetzwerk? uebungsszenario;
        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }


        #region UDP

        //Schnittstelle für LobbyScreenView im Konstruktor oder LobbyerstellenView am Ende
        public static async void BeginneZyklischesSendenVonLobbyinformation(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo)
        {
            if (udpClient != null) return;
            udpClient = new UdpClient(UDP_PORT);
            periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                string netzwerkBeitrittInfoAsString = netzwerkBeitrittInfo.Lobbyname.Replace("\t", "") + '\t'
                    + netzwerkBeitrittInfo.Protokoll.Replace("\t", "") + '\t'
                    + netzwerkBeitrittInfo.Variante.Replace("\t", "") + '\t'
                    + netzwerkBeitrittInfo.Schwierigkeitsgrad.ToString().Replace("\t", "") + '\t';
                //TODO hier noch die Rollen hinzufügen
                /*
                + netzwerkBeitrittInfo.Protokoll.Replace("\t", "") + '\t'
                + netzwerkBeitrittInfo.Protokoll.Replace("\t", "") + '\t'
                + netzwerkBeitrittInfo.Protokoll.Replace("\t", "");*/

                byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(netzwerkBeitrittInfoAsString);
                byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
                nachrichtZumSenden[0] = LOBBYINFORMATION;
                Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
                try
                {
                    udpClient.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", UDP_PORT);
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Senden vom Host geworfen"); break; }

            }
            ErstelleTCPLobby();
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
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht, Rolle? empfänger = null)
        {
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
            nachrichtZumSenden[0] = commandIdentifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            if (empfänger == null)
            {
                foreach(NetworkStream networkStream in networkStreams)
                {
                    networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                }
            }
            else
            {
                NetworkStream networkStream = rolleNetworkStreams[empfänger];
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
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        NetworkStream networkStream = tcpClient.GetStream();
                        networkStreams.Add(networkStream);
                        StarteTCPListeningThread(networkStream);
                    }
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Verbindung Annehmen als Host geworfen"); }
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
            //Hier ist man Server und man muss die neue Information an alle seine aktiven Verbindungen schicken
        }

        //Schnittstelle für LobbyScreenView
        public static void StarteUebungsszenario()
        {
            //Hier ist man Server und man muss die Information an alle seine aktiven Verbindungen schicken
            //Hier zyklisches Senden Beenden
        }

        //Schnittstelle fürs Übungsszenario
        public static void UebergebeKontrolle(Rolle nächsteRolle)
        {
            //Hier ist man Server und man muss die Information an eine gezielte Verbindung schicken
        }

        //Schnittstelle fürs Übungsszenario
        public static void SendeAufzeichnungsUpdate(Rolle empfänger, List<Handlungsschritt> neueHandlungsschritte)
        {
            //Hier ist man Server und man muss die Informationen an eine gezielte Verbindung schicken
        }

        //Schnittstelle fürs Übungsszenario
        public static void BeendeUebungsszenario()
        {
            //Hier kann man beides sein und je nachdem muss man:
            //Als Server: - gezielt an die andere Verbindung die Info schicken
            //Als Client: - nix mehr machen
            //Als beide: - Die Aufzeichnung anzeigen?
        }

        private static void StarteTCPListeningThread(NetworkStream networkStream)
        {
            new Thread(() =>
            {
                byte[] kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                //Network Streams in irgendeine Listenform bringen um sie schön wieder schließen zu können?
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
                                            break;
                                        case RolleEnum.Bob:
                                            bobRolle = new Rolle(RolleEnum.Bob, empfangeneNachrichtTeile[1]);
                                            break;
                                        case RolleEnum.Eve:
                                            eveRolle = new Rolle(RolleEnum.Eve, empfangeneNachrichtTeile[1]);
                                            break;
                                            //Notify Changed?
                                    }
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
                                            //Notify Changed?
                                    }
                                }
                                break;
                            case ZUG_BEENDEN:
                                //Hier ist man Server und man hat von einem Client in seiner Lobby die Information erhalten, dass dieser fertig ist
                                //Handlungsschritte zum Übungsszenario weitergeben
                                //TODO implementieren
                                uebungsszenario?.ZugWurdeBeendet(new List<Handlungsschritt>());
                                break;
                            case UEBUNGSSZENARIO_ENDE:
                                uebungsszenario?.UebungsszenarioWurdeBeendetClient();
                                break;
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen mit folgender Adresse geworfen: "); break; }
                }
            }).Start();
        }

        #endregion

    }
}
