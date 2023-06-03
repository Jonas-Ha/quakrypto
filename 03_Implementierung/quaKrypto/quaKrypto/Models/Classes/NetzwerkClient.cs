using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    public static class NetzwerkClient
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

        private const int UDP_PORT = 18523;
        private const int TCP_PORT = 32581;

        private static UdpClient? udpClient = null;
        private static TcpClient? tcpClient = null;

        private const int TCP_RECEIVE_BUFFER_SIZE = 8192;

        private static NetworkStream? networkStream = null;

        private static Dictionary<IPAddress, UebungsszenarioNetzwerkBeitrittInfo> verfügbareLobbys = new();

        //Schnittstelle für Lobby Beitreten
        public static ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbareLobbys
        {
            get
            {
                ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> returnCollection = new();
                foreach (UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo in verfügbareLobbys.Values)
                {
                    returnCollection.Add(netzwerkBeitrittInfo);
                }
                return returnCollection;
            }
        }

        private static UebungsszenarioNetzwerk? uebungsszenario;
        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }

        #region UDP
        //Schnittstelle LobyyBeitrittView
        public static void BeginneSucheNachLobbys()
        {
            new Thread(() =>
            {
                if (udpClient != null) return;
                udpClient = new UdpClient(UDP_PORT);
                IPEndPoint senderAdresse = new(0, 0);
                while (true)
                {
                    try
                    {
                        byte[] kompletteNachrichtAlsBytes = udpClient.Receive(ref senderAdresse);
                        byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                        if (commandIdentifier == LOBBYINFORMATION)
                        {
                            string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                            if (Enum.TryParse(empfangeneNachrichtTeile[3], out SchwierigkeitsgradEnum schwierigkeit))
                            {
                                //Rollen noch klären
                                UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo = new(senderAdresse.Address, empfangeneNachrichtTeile[0], empfangeneNachrichtTeile[1], empfangeneNachrichtTeile[2], schwierigkeit);
                                verfügbareLobbys.Add(senderAdresse.Address, netzwerkBeitrittInfo);
                                //NOTIFY CHANGED?
                            }
                        }
                        else if (commandIdentifier == LOBBY_NICHT_MEHR_VERFUEGBAR)
                        {
                            verfügbareLobbys.Remove(senderAdresse.Address);
                            //NOTIFY CHANGED?
                        }
                    }
                    catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Empfangen vom Client geworfen"); break; }
                }
            }).Start();
        }

        //Schnittstelle zu LobbyBeitrittView
        public static void BeendeSucheNachLobbys()
        {
            udpClient?.Close();
            udpClient = null;
        }

        #endregion

        #region TCP
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht)
        {
            if (networkStream == null) return;
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
            nachrichtZumSenden[0] = commandIdentifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
        }

        //Schnittstelle mit Lobby Beitreten
        public static bool VerbindeMitUebungsszenario(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo)
        {
            if (tcpClient != null) return false;
            tcpClient = new TcpClient(netzwerkBeitrittInfo.IPAddress.ToString(), TCP_PORT);
            networkStream = tcpClient.GetStream();
            StarteTCPListeningThread(networkStream);
            BeendeSucheNachLobbys();
            return true;
        }

        private static void TrenneVerbindungMitUebungsszenario()
        {
            networkStream?.Close();
            tcpClient?.Close();
            networkStream = null;
            tcpClient = null;
        }

        //Schnittstelle LobbyScreenView
        public static void WaehleRolle(RolleEnum gewählteRolle, string alias)
        {
            SendeNachrichtTCP(ROLLE_WAEHLEN, gewählteRolle.ToString() + '\t' + alias.Replace("\t",""));
        }

        //Schnittstelle LobbyScreenView
        public static void GebeRolleFrei(RolleEnum freizugebendeRolle)
        {
            SendeNachrichtTCP(ROLLE_FREIGEBEN, freizugebendeRolle.ToString());
        }

        //Schnittstelle für Übungsszenario
        public static void BeendeZug(List<Handlungsschritt> handlungsschritte)
        {
            //TODO noch zu implementieren
            SendeNachrichtTCP(ZUG_BEENDEN, "TODO");
        }

        //Schnittstelle fürs Übungsszenario
        public static void BeendeUebungsszenario()
        {
            SendeNachrichtTCP(UEBUNGSSZENARIO_ENDE, "");
            TrenneVerbindungMitUebungsszenario();
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
                            case ROLLENINFORMATION:
                                //Hier ist man Client und muss die neue Information Irgendwie nach außen an die View tragen
                                //An das Übungsszenario die neuen Rolleninformationen übergeben
                                break;
                            case UEBUNGSSZENARIO_STARTEN:
                                //Hier ist man Client und man muss nach Außen tragen, dass das Übungsszenario beginnt
                                break;
                            case KONTROLLE_UEBERGEBEN:
                                //Hier ist man Client und man muss nach Außen tragen, dass man nun an der Reihe ist
                                break;
                            case AUFZEICHNUNG_UPDATE:
                                //Hier ist man Client und man muss seine Aufzeichnung mit den neuesten Handlungsschritten updaten
                                break;
                            case UEBUNGSSZENARIO_ENDE:
                                //An das Uebungsszenario weitergeben
                                //TrenneVerbindungMitUebungsszenario()
                                //Aufzeichnung anzeigen
                                break;
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen im Client mit dem Host geworfen"); break; }
                }
            }).Start();
        }
        #endregion
    }
}
