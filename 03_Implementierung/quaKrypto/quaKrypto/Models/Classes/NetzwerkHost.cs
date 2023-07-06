// **********************************************************
// File: NetzwerkHost.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

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
using System.Windows;
using System.Xml.Serialization;

namespace quaKrypto.Models.Classes
{
    //Diese statische Klasse stellt funktionen für einen Host im Netzwerk bereit.
    public static class NetzwerkHost
    {
        //Dies sind die Indentifier für die Kommunikation über das Netzwerk
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

        //Das sind die vorher ausgemachten Ports, um eine Kommunikation zu erstellen.
        private const int UDP_LISTEN_PORT = 18523;
        private const int UDP_SEND_PORT = 18524;

        //Das sind die Member, welche für eine Kommunikation benötigt werden.
        private static UdpClient? udpClient = null;
        private static TcpListener? tcpListener = null;

        private const int TCP_RECEIVE_BUFFER_SIZE = 131072;

        //Hier werden die einzelnen Network-Streams von den jeweiligen Rollen gespeichert.
        private static readonly Dictionary<RolleEnum, NetworkStream> rolleNetworkStreams = new();
        private static readonly List<NetworkStream> networkStreams = new();

        //Das sind die einzelnen Rollen, welche in einem Übungsszenario vorkommen können.
        private static Rolle? aliceRolle, bobRolle, eveRolle;
        public static Rolle? AliceRolle { get { return aliceRolle; } set { aliceRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.AliceState = aliceRolle != null; } }
        public static Rolle? BobRolle { get { return bobRolle; } set { bobRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.BobState = bobRolle != null; } }
        public static Rolle? EveRolle { get { return eveRolle; } set { eveRolle = value; if (uebungsszenarioNetzwerkBeitrittInfo != null) uebungsszenarioNetzwerkBeitrittInfo.EveState = eveRolle != null; } }

        //Hier wird das Übungsszenario abgespeichert, um funktionen aufzurufen, wenn bestimmte Befehle über das Netzwerk gesendet werden.
        private static UebungsszenarioNetzwerk? uebungsszenario;
        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }

        //Das ist die Netzwerkbeitrittsinfo des Hosts, welche er über das Netzwerk sendet.
        private static UebungsszenarioNetzwerkBeitrittInfo? uebungsszenarioNetzwerkBeitrittInfo;

        //Das ist eine Sperrvariable, welche ein gezieltes Herunterfahren des Netzwerkes garantiert.
        public static bool BeendenErlaubt { get; set; } = true;

        //Diese Methode setzt die Netzwerkklasse wieder zurück, nachdem ein Spiel z.B. abgeschlossen wurde.
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

        //Diese Methode stößt das Senden der eigenen Lobby an.
        //Es wird ein neuer Thread gestartet, welcher einen UDP-Client auf einem verfügbaren Port erstellt, um die Lobby an die Clients zu bringen.
        public static async void BeginneZyklischesSendenVonLobbyinformation(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo, int portToSendTo = UDP_LISTEN_PORT)
        {
            if (udpClient != null) return;
            uebungsszenarioNetzwerkBeitrittInfo = netzwerkBeitrittInfo;
            ErstelleTCPLobby();
            udpClient = new UdpClient(UDP_SEND_PORT);
            using PeriodicTimer periodicTimer = new(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
            //Hier wird jeweils eine Sekunde zwischen dem Senden von Lobbyinformation gewartet, um einer Überlastung des Netzwerks vorzubeugen.
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
                    //Hier wird die Information an die UDP-Broadcast-Adresse geschickt, sodass alle Clients in dem Netzwerk die Information bekommen.
                    udpClient?.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", portToSendTo);
                }
                catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Senden vom Host geworfen"); break; }
            }
            BeendeZyklischesSendenVonLobbyinformation();
        }

        //Diese Methode beendet das zyklische Senden der Lobbyinformation.
        public static void BeendeZyklischesSendenVonLobbyinformation()
        {
            if (udpClient == null) return;
            try
            {
                //Am Ende wird den Clients noch mitgeteilt, dass diese Lobby nun nicht mehr verfügbar ist.
                udpClient?.Send(new byte[] { LOBBY_NICHT_MEHR_VERFUEGBAR }, 1, "255.255.255.255", UDP_LISTEN_PORT);
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
            }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #region TCP
        //Dies ist eine interne Hilfsmethode, um eine Nachricht über TCP zu senden. Es wird der Identifier und die Nachricht angegeben und diese Nachricht wird dann gesendet.
        //Außerdem kann ein Empfänger angegeben werden, wenn man gezielt nur einem Client eine Nachricht senden möchte.
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
                    //Hier wird sich die alte Adresse gespeichert, um nicht zweimal einem Client dieselbe Nachricht zu senden.
                    IPAddress? savedIPAddress = null;
                    foreach (NetworkStream networkStream in rolleNetworkStreams.Values)
                    {
                        IPEndPoint? endPoint = (IPEndPoint?)networkStream.Socket.LocalEndPoint;
                        if (savedIPAddress != null && endPoint != null && savedIPAddress == endPoint.Address) break;
                        networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                        savedIPAddress = endPoint?.Address;
                    }
                }
                else
                {
                    //Hier wurde kein Empfänger angegeben und somit wird jeder Rolle die Nachricht gesendet.
                    NetworkStream networkStream = rolleNetworkStreams[(RolleEnum)empfänger];
                    networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
                }
            }
            catch (Exception) { }
        }

        //Diese Methode wird verwendet, um eine TCP-Lobby zu erstellen, welche Clients aufnimmt.
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
                        //Der Host akzeptiert neue Clients und gibt den NetworkStream weiter an eine ander Funktion.
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();

                        NetworkStream networkStream = tcpClient.GetStream();
                        networkStreams.Add(networkStream);
                        StarteTCPListeningThread(networkStream);
                        Thread.Sleep(100);
                        SendeRollenInformation();
                    }
                }
                catch (SocketException) { BeendeTCPLobby(); Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Verbindung Annehmen als Host geworfen"); }
            }).Start();
        }

        //Diese Methode wird verwendet, um eine bestehende Lobby gezielt zu beenden.
        public static void BeendeTCPLobby()
        {
            if (uebungsszenario==null || !uebungsszenario.Beendet)
            {
                SendeNachrichtTCP(LOBBY_NICHT_MEHR_VERFUEGBAR, "");
            }
            tcpListener?.Stop();
            tcpListener = null;
            //Daraufhin werden alle vorhandenen Networkstreams geschlossen.
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

        //Diese Methode wird aufgerufen, wenn es Änderungen an den Rollen gibt.
        public static void SendeRollenInformation()
        {
            SendeNachrichtTCP(ROLLENINFORMATION, (AliceRolle == null ? "" : AliceRolle.Alias) + '\t' + (BobRolle == null ? "" : BobRolle?.Alias) + '\t' + (EveRolle == null ? "" : EveRolle?.Alias));
        }

        //Diese Methode wird aufgerufen, wenn das Übungsszenario beginnen kann.
        //Hier wird auch der Seed übertragen, welcher wichtig ist, wenn das Übungsszenario Information vorgenerieren soll.
        public static void StarteUebungsszenario(RolleEnum startRolle, int seed)
        {
            SendeNachrichtTCP(UEBUNGSSZENARIO_STARTEN, $"{startRolle}\t{seed}");
            BeendeZyklischesSendenVonLobbyinformation();
        }

        //Hier übergibt der Host die Kontrolle an einen Client, welcher übergeben werden kann.
        public static void UebergebeKontrolle(RolleEnum nächsteRolle) => SendeNachrichtTCP(KONTROLLE_UEBERGEBEN, nächsteRolle.ToString(), nächsteRolle);

        //Hier gibt es neue Handlungsschritte, welche den Clients mitgeteilt werden müssen.
        public static void SendeAufzeichnungsUpdate(List<Handlungsschritt> neueHandlungsschritte, RolleEnum? empfänger = null)
        {
            if (empfänger != null && rolleNetworkStreams.Count == 2 && rolleNetworkStreams.ElementAt(0).Value.Equals(rolleNetworkStreams.ElementAt(1).Value)) return;
            XmlSerializer xmlSerializer = new(typeof(List<Handlungsschritt>));
            using StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, neueHandlungsschritte);
            SendeNachrichtTCP(AUFZEICHNUNG_UPDATE, stringWriter.ToString(), empfänger);
        }

        //Hier wird das Übungsszenario beendet und damit auch die TCP-Lobby
        public static void BeendeUebungsszenario()
        {
            if (!BeendenErlaubt) return;
            SendeNachrichtTCP(UEBUNGSSZENARIO_ENDE, "");
            BeendeTCPLobby();
        }

        //Das ist der TCP Listening Thread. Dieser kümmert sich im Hintergrund um die eingehenden Nachrichten des Clients und handelt entsprechend.
        private static void StarteTCPListeningThread(NetworkStream networkStream)
        {
            new Thread(() =>
            {
                byte[] kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                while (true)
                {
                    try
                    {
                        //Hier wird eine neue Nachricht von einem Client empfangen.
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
                                //Hier hat sich ein Client für eine Rolle entschieden.
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
                                //Hier hat ein Client eine Rolle wieder freigegeben.
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
                                //Hier hat ein Client seinen Zug beendet und es gibt neue Handlungsschritte, welche an die Aufzeichnung angehängt werden müssen.
                                case ZUG_BEENDEN:
                                    List<Handlungsschritt> listeEmpfangenerHandlungsschritte = new();
                                    XmlSerializer xmlHandlungsschrittSerializer = new(typeof(List<Handlungsschritt>));
                                    using (StringReader stringReader = new(empfangeneNachrichtTeile[0]))
                                    {
                                        object? deserialisiertesObjekt = xmlHandlungsschrittSerializer.Deserialize(stringReader);
                                        if (deserialisiertesObjekt != null)
                                        {
                                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                                uebungsszenario?.ZugWurdeBeendet((List<Handlungsschritt>)deserialisiertesObjekt);
                                            }));    
                                        }
                                    }
                                    break;
                                //Hier hat sich ein Client dazu entschlossen, das Übungsszenario zu beenden.
                                case UEBUNGSSZENARIO_ENDE:
                                    uebungsszenario?.Beenden();
                                    break;
                            }
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (Exception)
                    {
                        //Wenn ein Fehler geworfen wird, ist es wichtig, dass der Host weiß, welche Rolle den Fehler geworfen hat, um dementsprechend handeln zu können.
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
