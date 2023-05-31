// **********************************************************
// File: Netzwerk.cs
// Autor: Daniel Hannes
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

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
using quaKrypto.Models.Interfaces;

namespace quaKrypto.Models.Classes
{
    static class Netzwerk
    //Quelle: https://stackoverflow.com/questions/40616911/c-sharp-udp-broadcast-and-receive-example
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

        private const int UDP_LISTENINGPORT = 18523;
        private const int TCP_PORT = 32581;

        private static UdpClient udpClient = new(UDP_LISTENINGPORT);
        private static TcpClient? tcpClient = null;
        private static NetworkStream? networkStreamMitServer = null;

        private static bool SENDEN_UDP_AKTIV = false;
        private static bool EMPFANGEN_UDP_AKTIV = false;

        private static IPAddress? IP_AKTUELLES_UEBUNGSSZENARIO = null;

        private static Dictionary<IPAddress, IUebungsszenario> UEBUNGSSZENARIEN_NACH_IP_ADRESSE = new();

        #region UDP
        private static void SendeNachrichtUDP(byte identifier, string nachricht)
        {
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
            nachrichtZumSenden[0] = identifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            udpClient.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", UDP_LISTENINGPORT);
        }
        public static void BeginneSucheNachLobbys()
        {
            if (!EMPFANGEN_UDP_AKTIV)
            {
                EMPFANGEN_UDP_AKTIV = true;
                new Thread(() =>
                {
                    IPEndPoint senderAdresse = new(0, 0);
                    while (true)
                    {
                        try
                        {
                            byte[] kompletteNachrichtAlsBytes = udpClient.Receive(ref senderAdresse);
                            byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                            string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                            string lobbyName = empfangeneNachrichtTeile[0];
                            IVariante variante = null;                                                                            //Hier muss ich auf implementierungen von Varianten warten
                            if (!Enum.TryParse(empfangeneNachrichtTeile[1], out SchwierigkeitsgradEnum schwierigkeitsgrad)) schwierigkeitsgrad = SchwierigkeitsgradEnum.leicht; //Default leicht?
                            List<Rolle> rollen = new();
                            IUebungsszenario empfangenesUebungsszenario = new UebungsszenarioNetzwerk(schwierigkeitsgrad, variante, 0, 0); //Wo ist der Name und wo sind die Rollen?
                            if (commandIdentifier == LOBBYINFORMATION)
                            {
                                UEBUNGSSZENARIEN_NACH_IP_ADRESSE[senderAdresse.Address] = empfangenesUebungsszenario;
                                //NOTIFY CHANGED?
                            }
                            else if (commandIdentifier == LOBBY_NICHT_MEHR_VERFUEGBAR)
                            {
                                UEBUNGSSZENARIEN_NACH_IP_ADRESSE.Remove(senderAdresse.Address);
                                if (UEBUNGSSZENARIEN_NACH_IP_ADRESSE.Count == 0) IP_AKTUELLES_UEBUNGSSZENARIO = null;
                                //NOTIFY CHANGED?
                            }
                        }
                        catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Empfangen geworfen"); break; }
                    }
                }).Start();
            }
        }

        public static void BeendeSucheNachLobbys()
        {
            if (EMPFANGEN_UDP_AKTIV)
            {
                EMPFANGEN_UDP_AKTIV = false;
                udpClient.Close();
                udpClient = new(UDP_LISTENINGPORT);
            }
        }

        public static async void BeginneZyklischesSendenVonLobbyinformation(IUebungsszenario uebungsszenario)
        {
            if (!SENDEN_UDP_AKTIV)
            {
                SENDEN_UDP_AKTIV = true;
                periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    SendeAktuelleLobbyinformationMitCommandIdentifier(LOBBYINFORMATION);
                }
            }
        }

        public static void BeendeZyklischesSendenVonLobbyinformation()
        {
            if (SENDEN_UDP_AKTIV)
            {
                SENDEN_UDP_AKTIV = false;
                SendeAktuelleLobbyinformationMitCommandIdentifier(LOBBY_NICHT_MEHR_VERFUEGBAR);
                periodicTimer?.Dispose();
            }
        }

        private static void SendeAktuelleLobbyinformationMitCommandIdentifier(byte commandIdentifier)
        {
            if (IP_AKTUELLES_UEBUNGSSZENARIO != null)
            {
                IUebungsszenario zuSendendesUebungsszenario = UEBUNGSSZENARIEN_NACH_IP_ADRESSE[IP_AKTUELLES_UEBUNGSSZENARIO];
                SendeNachrichtUDP(commandIdentifier,
                    zuSendendesUebungsszenario.Name.Replace("\t", "") + '\t' +
                    zuSendendesUebungsszenario.Variante + '\t' +                                        //Variante noch richtig parsen
                    zuSendendesUebungsszenario.Schwierigkeitsgrad.ToString().Replace("\t", "") + '\t' +
                    zuSendendesUebungsszenario.Rollen);                                                 //Hier die Rollen Noch Parsen einzelnd und Tab replacen?
            }
        }

        #endregion

        #region TCP
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht)
        {
            //An wen schicken? Als Client an deinen Server aber als Server an wen?
        }

        //Nur von Client aus aufrufen!
        public static void VerbindeMitAktuellerLobby()
        {
            if (tcpClient != null) TrenneVerbindungMitAktuellerLobby(); //Evtl. Besser machen?
            if (IP_AKTUELLES_UEBUNGSSZENARIO == null) throw new Exception("Es ist aktuell keine Lobby ausgewählt");
            tcpClient = new TcpClient(IP_AKTUELLES_UEBUNGSSZENARIO.ToString(), TCP_PORT);
            networkStreamMitServer = tcpClient.GetStream();
        }

        //Nur von Client aus aufrufen!
        public static void TrenneVerbindungMitAktuellerLobby()
        {
            networkStreamMitServer?.Close();
            tcpClient?.Close();
            networkStreamMitServer = null;
            tcpClient = null;
        }

        //Nur als Lobby-Host aufrufen!
        public static void ErstelleTCPLobby()
        {
            new Thread(() =>
            {
                try
                {
                    TcpListener tcpListener = new(IPAddress.Any, TCP_PORT);
                    tcpListener.Start();
                    while (true)
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        StarteTCPListeningThreadMitTCPClient(tcpClient);
                        //Hier noch ein SendingThread?
                    }
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen als Host geworfen"); }
            }).Start();
        }

        //Nur als Lobby-Host aufrufen!
        public static void BeendeTCPLobby()
        {
            //Hier tcp listener beenden und streams auch beenden?
        }

        public static void SendeRollenInformation()
        {
            //Hier ist man Server und man muss die neue Information an alle seine aktiven Verbindungen schicken
        }
        public static void WaehleRolle()
        {
            //Hier ist man Client und man muss die gewählte Rolle an seine Server-Verbindung schicken
        }
        public static void GebeRolleFrei()
        {
            //Hier ist man Client und man muss die gewählte Rolle an seine Server-Verbindung schicken
        }
        public static void StarteUebungsszenario()
        {
            //Hier ist man Server und man muss die Information an alle seine aktiven Verbindungen schicken
        }
        public static void UebergebeKontrolle()
        {
            //Hier ist man Server und man muss die Information an eine gezielte Verbindung schicken
        }
        public static void BeendeZug()
        {
            //Hier ist man Client und man muss die Informationen an seine Server-Verbindung schicken
        }
        public static void SendeAufzeichnungsUpdate()
        {
            //Hier ist man Server und man muss die Informationen an eine gezielte Verbindung schicken
        }
        public static void BeendeUebungsszenario()
        {
            //Hier kann man beides sein und je nachdem muss man:
            //Als Server: - gezielt an die andere Verbindung die Info schicken
            //Als Client: - nix mehr machen
            //Als beide: - Die Aufzeichnung anzeigen?
        }


        private static void StarteTCPListeningThreadMitTCPClient(TcpClient tcpClient)
        {
            new Thread(() =>
            {
                byte[] kompletteNachrichtAlsBytes = new byte[tcpClient.ReceiveBufferSize];
                NetworkStream networkStream = tcpClient.GetStream();
                //Network Streams in irgendeine Listenform bringen um sie schön wieder schließen zu können?
                while (true)
                {
                    try
                    {
                        networkStream.Read(kompletteNachrichtAlsBytes, 0, tcpClient.ReceiveBufferSize);
                        byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                        string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                        switch (commandIdentifier)
                        {
                            case ROLLENINFORMATION:
                                //Hier ist man Client und muss die neue Information Irgendwie nach außen an die View tragen
                                break;
                            case ROLLE_WAEHLEN:
                                //Hier ist man Server und muss die gewählte Information Nach Außen an das Übungsszenario(?) tragen
                                //Außerdem die neue Rolleninformation noch weitersenden
                                break;
                            case ROLLE_FREIGEBEN:
                                //Hier ist man Server und muss die gewählte Information Nach Außen an das Übungsszenario(?) tragen
                                //Außerdem die neue Rolleninformation noch weitersenden
                                break;
                            case UEBUNGSSZENARIO_STARTEN:
                                //Hier ist man Client und man muss nach Außen tragen, dass das Übungsszenario beginnt
                                break;
                            case KONTROLLE_UEBERGEBEN:
                                //Hier ist man Client und man muss nach Außen tragen, dass man nun an der Reihe ist
                                break;
                            case ZUG_BEENDEN:
                                //Hier ist man Server und man hat von einem Client in seiner Lobby die Information erhalten, dass dieser fertig ist
                                break;
                            case AUFZEICHNUNG_UPDATE:
                                //Hier ist man Client und man muss seine Aufzeichnung mit den neuesten Handlungsschritten updaten
                                break;
                            case UEBUNGSSZENARIO_ENDE:
                                //Hier kann man beides sein(?) auf jeden Fall soll man danach die aktuelle Aufzeichnung anzeigen
                                break;
                        }
                        kompletteNachrichtAlsBytes = new byte[tcpClient.ReceiveBufferSize];
                    }
                    catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen mit folgender Adresse geworfen: \"" + ((IPEndPoint?)tcpClient.Client.RemoteEndPoint)?.Address + "\'"); break; }
                }
            }).Start();
        }

        #endregion

    }
}
