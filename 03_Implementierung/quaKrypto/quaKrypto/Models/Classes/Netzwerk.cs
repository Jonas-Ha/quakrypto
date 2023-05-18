using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    static class Netzwerk
    //Quelle: https://stackoverflow.com/questions/40616911/c-sharp-udp-broadcast-and-receive-example
    {
        private static readonly byte LOBBYINFORMATION = 0x01;
        private static readonly byte LOBBY_NICHT_MEHR_VERFÜGBAR = 0x02;
        private static readonly byte ROLLENINFORMATION = 0x03;
        private static readonly byte ROLLE_WÄHLEN = 0x04;
        private static readonly byte ROLLE_FREIGEBEN = 0x05;
        private static readonly byte ÜBUNGSSZENARIO_STARTEN = 0x06;
        private static readonly byte KONTROLLE_ÜBERGEBEN = 0x07;
        private static readonly byte ZUG_BEENDEN = 0x08;
        private static readonly byte AUFZEICHNUNG_UPDATE = 0x09;
        private static readonly byte ÜBUNGSSZENARIO_ENDE = 0x10;

        private static readonly int ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS = 1000;
        private static PeriodicTimer? periodicTimer;

        private static readonly int UDP_LISTENINGPORT = 18523;
        private static readonly int TCP_PORT = 32581;

        private static UdpClient udpClient = new(UDP_LISTENINGPORT);

        private static bool SENDEN_UDP_AKTIV = false;
        private static bool EMPFANGEN_UDP_AKTIV = false;

        private static ILobby? AKTUELLE_LOBBY;


        private static void SendeNachrichtUDP(byte identifier, string nachricht)
        {
            byte[] nachrichtAlsByteArray = Encoding.UTF8.GetBytes(nachricht);
            byte[] nachrichtZumSenden = new byte[nachrichtAlsByteArray.Length + 1];
            nachrichtZumSenden[0] = identifier;
            Array.Copy(nachrichtAlsByteArray, 0, nachrichtZumSenden, 1, nachrichtAlsByteArray.Length);
            udpClient.Send(nachrichtZumSenden, nachrichtZumSenden.Length, "255.255.255.255", UDP_LISTENINGPORT);
        }

        public static void BeginneSucheNachLobbys() //Hier eventuell Objekt übergeben, welches die Lobby anzeigt
        {
            if (!EMPFANGEN_UDP_AKTIV)
            {
                EMPFANGEN_UDP_AKTIV = true;
                new Thread(() =>
                {
                    IPEndPoint ipEndPunkt = new(0, 0);
                    while (true)
                    {
                        try
                        {
                            //Hier Daten an die entsprechende Klasse übergeben, welche die Lobbys anzeigt
                            byte[] empfangeneDaten = udpClient.Receive(ref ipEndPunkt);
                            if (empfangeneDaten[0] == LOBBYINFORMATION)
                            {
                                Trace.WriteLine("Lobbyinformation empfangen: " + Encoding.UTF8.GetString(empfangeneDaten[1..]));
                                //Lobby hinzufügen/updaten
                            }
                            else if (empfangeneDaten[0] == LOBBY_NICHT_MEHR_VERFÜGBAR)
                            {
                                Trace.WriteLine("Lobby NICHT MEHR VERFÜGBAR: " + Encoding.UTF8.GetString(empfangeneDaten[1..]));
                                //Lobby löschen
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

        public static async void BeginneZyklischesSendenVonLobbyinformation(ILobby lobby)
        {
            if (!SENDEN_UDP_AKTIV)
            {
                SENDEN_UDP_AKTIV = true;
                AKTUELLE_LOBBY = lobby;
                periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(ZEIT_ZWISCHEN_LOBBYINFORMATION_SENDEN_IN_MS));
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    SendeNachrichtUDP(LOBBYINFORMATION, "NOCH NICHT IMPLEMENTIERT"); //AKTUELLE_LOBBY.Lobbyname + '\t' + AKTUELLE_LOBBY.Variante + '\t' + AKTUELLE_LOBBY.Schwierigkeitsgrad + '\t' + AKTUELLE_LOBBY.BesetzteRollen
                }
            }
        }

        public static void BeendeZyklischesSendenVonLobbyinformation()
        {
            if (SENDEN_UDP_AKTIV)
            {
                SENDEN_UDP_AKTIV = false;
                SendeNachrichtUDP(LOBBY_NICHT_MEHR_VERFÜGBAR, "NOCH NICHT IMPLEMENTIERT"); //AKTUELLE_LOBBY.Lobbyname + '\t' + AKTUELLE_LOBBY.Variante + '\t' + AKTUELLE_LOBBY.Schwierigkeitsgrad + '\t' + AKTUELLE_LOBBY.BesetzteRollen
                periodicTimer?.Dispose();
            }
        }
    }

}
