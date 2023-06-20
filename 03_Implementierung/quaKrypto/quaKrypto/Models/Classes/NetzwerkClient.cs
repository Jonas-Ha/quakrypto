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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

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
            get;
        } = new ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo>();

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
                        Trace.WriteLine("BeginneLobbySucheNachrichtErhalten");
                        byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                        if (commandIdentifier == LOBBYINFORMATION)
                        {
                            string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                            if (Enum.TryParse(empfangeneNachrichtTeile[3], out SchwierigkeitsgradEnum schwierigkeit))
                            {
                                bool aliceBesetzt = bool.Parse(empfangeneNachrichtTeile[4]);
                                bool bobBesetzt = bool.Parse(empfangeneNachrichtTeile[5]);
                                bool eveBesetzt = bool.Parse(empfangeneNachrichtTeile[6]);
                                UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo = new(senderAdresse.Address, empfangeneNachrichtTeile[0], empfangeneNachrichtTeile[1], empfangeneNachrichtTeile[2], schwierigkeit, aliceBesetzt, bobBesetzt, eveBesetzt) { StartPhase = uint.TryParse(empfangeneNachrichtTeile[7], out uint startPhase) ? startPhase : 0, EndPhase = uint.TryParse(empfangeneNachrichtTeile[8], out uint endPhase) ? endPhase : 5 };
                                if (!verfügbareLobbys.ContainsKey(senderAdresse.Address))
                                {
                                    verfügbareLobbys.Add(senderAdresse.Address, netzwerkBeitrittInfo);
                                    Application.Current.Dispatcher.Invoke(new Action(() => VerfuegbareLobbys.Add(netzwerkBeitrittInfo)));
                                }

                                //NOTIFY CHANGED?
                            }
                        }
                        else if (commandIdentifier == LOBBY_NICHT_MEHR_VERFUEGBAR)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => VerfuegbareLobbys.Remove(verfügbareLobbys[senderAdresse.Address])));
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

        public static void TrenneVerbindungMitUebungsszenario()
        {
            networkStream?.Close();
            tcpClient?.Close();
            networkStream = null;
            tcpClient = null;
        }

        //Schnittstelle LobbyScreenView
        public static void WaehleRolle(RolleEnum gewählteRolle, string alias)
        {
            SendeNachrichtTCP(ROLLE_WAEHLEN, gewählteRolle.ToString() + '\t' + alias.Replace("\t", ""));
        }

        //Schnittstelle LobbyScreenView
        public static void GebeRolleFrei(RolleEnum freizugebendeRolle)
        {
            SendeNachrichtTCP(ROLLE_FREIGEBEN, freizugebendeRolle.ToString());
        }

        //Schnittstelle für Übungsszenario
        public static void BeendeZug(List<Handlungsschritt> handlungsschritte)
        {
            string serializedHandlungsschritte = new("");
            XmlSerializer xmlSerializer = new(typeof(Handlungsschritt));
            for (int i = 0; i < handlungsschritte.Count; i++)
            {
                if (i != 0) serializedHandlungsschritte += '\t';
                using StringWriter stringWriter = new();
                xmlSerializer.Serialize(stringWriter, handlungsschritte[i]);
                serializedHandlungsschritte += stringWriter.ToString();
            }
            SendeNachrichtTCP(ZUG_BEENDEN, serializedHandlungsschritte);
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
                        string ales = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]);
                        string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                        switch (commandIdentifier)
                        {
                            case ROLLENINFORMATION:
                                Rolle? rolleAlice, rolleBob, rolleEve;
                                rolleAlice = empfangeneNachrichtTeile[0] == "" ? null : new Rolle(RolleEnum.Alice, empfangeneNachrichtTeile[0]);
                                rolleBob = empfangeneNachrichtTeile[1] == "" ? null : new Rolle(RolleEnum.Bob, empfangeneNachrichtTeile[1]);
                                empfangeneNachrichtTeile[2] = empfangeneNachrichtTeile[2].TrimEnd('\0');
                                rolleEve = empfangeneNachrichtTeile[2] == "" ? null : new Rolle(RolleEnum.Eve, empfangeneNachrichtTeile[2]);

                                uebungsszenario?.NeueRollenInformation(rolleAlice, rolleBob, rolleEve);
                                break;
                            case UEBUNGSSZENARIO_STARTEN:
                                uebungsszenario?.UebungsszenarioWurdeGestartet(Enum.TryParse(empfangeneNachrichtTeile[0], out RolleEnum startRolle) ? startRolle : RolleEnum.Alice);
                                break;
                            case KONTROLLE_UEBERGEBEN:
                                uebungsszenario?.KontrolleErhalten(Enum.TryParse(empfangeneNachrichtTeile[0], out RolleEnum nächsteRolle) ? nächsteRolle : RolleEnum.Alice);
                                break;
                            case AUFZEICHNUNG_UPDATE:
                                List<Handlungsschritt> listeEmpfangenerHandlungsschritte = new();
                                XmlSerializer xmlHandlungsschrittSerializer = new(typeof(Handlungsschritt));
                                foreach (string handlungsschritt in empfangeneNachrichtTeile)
                                {
                                    using StringReader stringReader = new StringReader(handlungsschritt);
                                    object? deserialisiertesObjekt = xmlHandlungsschrittSerializer.Deserialize(stringReader);
                                    if (deserialisiertesObjekt != null)
                                    {
                                        listeEmpfangenerHandlungsschritte.Add((Handlungsschritt)deserialisiertesObjekt);
                                    }
                                }
                                uebungsszenario?.AufzeichnungUpdate(listeEmpfangenerHandlungsschritte);
                                break;
                            case UEBUNGSSZENARIO_ENDE:
                                TrenneVerbindungMitUebungsszenario();
                                uebungsszenario?.Beenden();
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
