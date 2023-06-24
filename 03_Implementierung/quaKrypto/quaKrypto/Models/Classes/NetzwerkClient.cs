using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using quaKrypto.Models.Enums;
using System.Xml.Serialization;
using System.IO;
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

        private const int UDP_LISTEN_PORT = 18523;
        private const int UDP_SEND_PORT = 18524;

        private const int TCP_HOST_PORT = 32581;
        private const int TCP_CLIENT_PORT = 32582;

        private static UdpClient? udpClient = null;
        private static TcpClient? tcpClient = null;

        private const int TCP_RECEIVE_BUFFER_SIZE = 131072;

        private static NetworkStream? networkStream = null;

        private static Dictionary<IPAddress, UebungsszenarioNetzwerkBeitrittInfo> verfügbareLobbys = new();

        //Schnittstelle für Lobby Beitreten
        public static ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbareLobbys { get; } = new();

        private static UebungsszenarioNetzwerk? uebungsszenario;

        public static UebungsszenarioNetzwerk Ubungsszenario { set { uebungsszenario = value; } }

        #region UDP
        //Schnittstelle LobyyBeitrittView
        public static void BeginneSucheNachLobbys()
        {
            new Thread(() =>
            {
                if (udpClient != null) return;
                udpClient = new UdpClient(UDP_LISTEN_PORT);
                IPEndPoint senderAdresse = new(0, 0);
                while (true)
                {
                    try
                    {
                        if (udpClient == null) break;
                        byte[] kompletteNachrichtAlsBytes = udpClient.Receive(ref senderAdresse);
                        if (kompletteNachrichtAlsBytes.Length == 0) break;
                        byte commandIdentifier = kompletteNachrichtAlsBytes[0];
                        if (commandIdentifier == LOBBYINFORMATION)
                        {
                            string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes[1..]).Split('\t');
                            UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo = new(senderAdresse.Address, empfangeneNachrichtTeile[0], empfangeneNachrichtTeile[1], empfangeneNachrichtTeile[2], Enum.TryParse(empfangeneNachrichtTeile[3], out SchwierigkeitsgradEnum schwierigkeit) ? schwierigkeit : SchwierigkeitsgradEnum.Leicht, bool.TryParse(empfangeneNachrichtTeile[4], out bool aliceBesetzt) && aliceBesetzt, bool.TryParse(empfangeneNachrichtTeile[5], out bool bobBesetzt) && bobBesetzt, bool.TryParse(empfangeneNachrichtTeile[6], out bool eveBesetzt) && eveBesetzt) { StartPhase = uint.TryParse(empfangeneNachrichtTeile[7], out uint startPhase) ? startPhase : 0, EndPhase = uint.TryParse(empfangeneNachrichtTeile[8], out uint endPhase) ? endPhase : 5 };
                            if (!verfügbareLobbys.ContainsKey(senderAdresse.Address))
                            {
                                verfügbareLobbys.Add(senderAdresse.Address, netzwerkBeitrittInfo);
                                Application.Current.Dispatcher.Invoke(new Action(() => VerfuegbareLobbys.Add(netzwerkBeitrittInfo)));
                            }
                            else
                            {
                                verfügbareLobbys[senderAdresse.Address] = netzwerkBeitrittInfo;
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    int index = VerfuegbareLobbys.IndexOf(VerfuegbareLobbys.Where(lobby => lobby.IPAddress.Equals(senderAdresse.Address)).First());
                                    VerfuegbareLobbys[index].AliceState = netzwerkBeitrittInfo.AliceState;
                                    VerfuegbareLobbys[index].BobState = netzwerkBeitrittInfo.BobState;
                                    VerfuegbareLobbys[index].EveState = netzwerkBeitrittInfo.EveState;
                                }
                                ));
                            }
                        }
                        else if (commandIdentifier == LOBBY_NICHT_MEHR_VERFUEGBAR)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => { if (verfügbareLobbys.ContainsKey(senderAdresse.Address) && VerfuegbareLobbys.Any(l => l.IPAddress.Equals(senderAdresse.Address))) VerfuegbareLobbys.Remove(VerfuegbareLobbys.First(l => l.IPAddress.Equals(senderAdresse.Address))); }));
                            verfügbareLobbys.Remove(senderAdresse.Address);
                        }
                    }
                    catch (SocketException) { Trace.WriteLine("Eine Socket-Exception wurde beim UDP-Empfangen vom Client geworfen"); break; }
                }
                BeendeSucheNachLobbys();
            }).Start();
        }

        //Schnittstelle zu LobbyBeitrittView
        public static void BeendeSucheNachLobbys()
        {
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            verfügbareLobbys.Clear();
            Application.Current.Dispatcher.Invoke(new Action(() => { if (VerfuegbareLobbys.Count > 0) VerfuegbareLobbys.Clear(); }));
        }

        #endregion

        #region TCP
        private static void SendeNachrichtTCP(byte commandIdentifier, string nachricht)
        {
            if (networkStream == null) return;
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 4];
            nachrichtZumSenden[0] = commandIdentifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            for (int i = 0; i < 3; i++)
            {
                nachrichtZumSenden[^(1 + i)] = (byte)'\0';
            }
            networkStream.Write(nachrichtZumSenden, 0, nachrichtZumSenden.Length);
        }

        //Schnittstelle mit Lobby Beitreten
        public static bool VerbindeMitUebungsszenario(UebungsszenarioNetzwerkBeitrittInfo netzwerkBeitrittInfo)
        {
            if (tcpClient != null) return false;
            tcpClient = new TcpClient(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork), TCP_CLIENT_PORT));
            tcpClient.Connect(netzwerkBeitrittInfo.IPAddress, TCP_HOST_PORT);
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
            XmlSerializer xmlSerializer = new(typeof(List<Handlungsschritt>));
            using StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, handlungsschritte);
            SendeNachrichtTCP(ZUG_BEENDEN, stringWriter.ToString());
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
                        string[] empfangeneGanzeNachrichten = Encoding.UTF8.GetString(kompletteNachrichtAlsBytes).Split("\0\0\0");
                        foreach (string ganzeNachricht in empfangeneGanzeNachrichten)
                        {
                            if (ganzeNachricht == "") break;
                            byte commandIdentifier = (byte)ganzeNachricht[0];
                            string[] empfangeneNachrichtTeile = ganzeNachricht[1..].Split('\t');
                            for (int i = 0; i < empfangeneNachrichtTeile.Length; i++) empfangeneNachrichtTeile[i] = empfangeneNachrichtTeile[i].TrimEnd('\0');
                            switch (commandIdentifier)
                            {
                                case ROLLENINFORMATION:
                                    Rolle? rolleAlice, rolleBob, rolleEve;
                                    rolleAlice = empfangeneNachrichtTeile[0] == "" ? null : new Rolle(RolleEnum.Alice, empfangeneNachrichtTeile[0], "");
                                    rolleBob = empfangeneNachrichtTeile[1] == "" ? null : new Rolle(RolleEnum.Bob, empfangeneNachrichtTeile[1], "");
                                    empfangeneNachrichtTeile[2] = empfangeneNachrichtTeile[2].TrimEnd('\0');
                                    rolleEve = empfangeneNachrichtTeile[2] == "" ? null : new Rolle(RolleEnum.Eve, empfangeneNachrichtTeile[2], "");

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
                                    XmlSerializer xmlHandlungsschrittSerializer = new(typeof(List<Handlungsschritt>));
                                    using (StringReader stringReader = new StringReader(empfangeneNachrichtTeile[0]))
                                    {
                                        object? deserialisiertesObjekt = xmlHandlungsschrittSerializer.Deserialize(stringReader);
                                        if (deserialisiertesObjekt != null)
                                        {
                                            uebungsszenario?.AufzeichnungUpdate((List<Handlungsschritt>)deserialisiertesObjekt);
                                        }
                                    }
                                    break;
                                case UEBUNGSSZENARIO_ENDE:
                                    TrenneVerbindungMitUebungsszenario();
                                    uebungsszenario?.Beenden();
                                    break;
                            }
                        }
                        kompletteNachrichtAlsBytes = new byte[TCP_RECEIVE_BUFFER_SIZE];
                    }
                    catch (IOException) { Trace.WriteLine("Eine Socket-Exception wurde beim TCP-Empfangen im Client mit dem Host geworfen"); break; }
                }
            }).Start();
        }
        #endregion
    }
}
