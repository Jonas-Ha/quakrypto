//**************************
// File: NetzwerkHost_UnitTest.cs
// Autor: Daniel Hannes
// erstellt am: 05.06.2023
// Projekt: TestLibrary
//**************************

using NUnit.Framework;
using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;

namespace TestLibrary
{
    [TestFixture]
    public class NetzwerkHost_UnitTest
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

        private const int TESTPORT = 26573;

        #region UDP
        [Test]
        public void BeginneZyklischesSendenVonLobbyinformation_NetzwerkbeitrittinfoUebergeben_NetzwerkbeitrittinfoKommtAn()
        {
            //Arange
            const string ERROR = "ERROR";
            const string LOBBYNAME = "Name der Lobby";
            const string PROTOKOLLNAME = "Name des Protokolls";
            const string VARIANTENNAME = "Name der Variante";
            const bool ALICESTATE = true, BOBSTATE = false, EVESTATE = false;
            UebungsszenarioNetzwerkBeitrittInfo gesendeteNetzwerkBeitrittInfo = new(IPAddress.Any, LOBBYNAME, PROTOKOLLNAME, VARIANTENNAME, SchwierigkeitsgradEnum.leicht, ALICESTATE, BOBSTATE, EVESTATE);
            UebungsszenarioNetzwerkBeitrittInfo empfangeneNetzwerkBeitrittInfo;
            UdpClient udpClient = new(TESTPORT);
            IPEndPoint iPEndPoint = new(0, 0);
            byte[] empfangeneNachrichtBuffer;


            //Act
            NetzwerkHost.BeginneZyklischesSendenVonLobbyinformation(gesendeteNetzwerkBeitrittInfo, TESTPORT);
            empfangeneNachrichtBuffer = udpClient.Receive(ref iPEndPoint);
            string[] empfangeneNachrichtTeile = Encoding.UTF8.GetString(empfangeneNachrichtBuffer[1..]).Split('\t');
            if (Enum.TryParse(empfangeneNachrichtTeile[3], out SchwierigkeitsgradEnum schwierigkeit))
            {
                bool aliceBesetzt = bool.Parse(empfangeneNachrichtTeile[4]);
                bool bobBesetzt = bool.Parse(empfangeneNachrichtTeile[5]);
                bool eveBesetzt = bool.Parse(empfangeneNachrichtTeile[6]);
                empfangeneNetzwerkBeitrittInfo = new(IPAddress.Any, empfangeneNachrichtTeile[0], empfangeneNachrichtTeile[1], empfangeneNachrichtTeile[2], schwierigkeit, aliceBesetzt, bobBesetzt, eveBesetzt);
            }
            else
            {
                empfangeneNetzwerkBeitrittInfo = new(iPEndPoint.Address, ERROR, ERROR, ERROR, SchwierigkeitsgradEnum.schwer, false, false, false);
            }

            //Assert
            Assert.AreEqual(empfangeneNachrichtBuffer[0], LOBBYINFORMATION);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.Lobbyname, empfangeneNetzwerkBeitrittInfo.Lobbyname);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.Protokoll, empfangeneNetzwerkBeitrittInfo.Protokoll);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.Variante, empfangeneNetzwerkBeitrittInfo.Variante);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.Schwierigkeitsgrad, empfangeneNetzwerkBeitrittInfo.Schwierigkeitsgrad);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.AliceState, empfangeneNetzwerkBeitrittInfo.AliceState);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.BobState, empfangeneNetzwerkBeitrittInfo.BobState);
            Assert.AreEqual(gesendeteNetzwerkBeitrittInfo.EveState, empfangeneNetzwerkBeitrittInfo.EveState);
        }

        #endregion

        #region TCP

        public void SendeRollenInformation_NichtsMachen_DreimalNullKommtAn()
        {
            //Arrange


            //Act


            //Assert

        }

        public void StarteUebungsszenario_UebungsszenarioStartenKommtAn()
        {
            //Arrange



            //Act


            //Assert

        }

        public void UebergebeKontrolle_RolleKeineAhnung_UebergebeKontrolleKommtAn()
        {
            //Arrange


            //Act


            //Assert

        }

        public void SendeAufzeichnungsUpdate_AnRolleKeineAhnung_ListeMitHandlungsschrittenKommtan()
        {
            //Arrange


            //Act


            //Assert

        }

        public void BeendeUebungsszenario_NichtsMachen_UebungsszenarioEndeKommtBeiAllenRollenAn()
        {
            //Arrange


            //Act


            //Assert

        }

        #endregion
    }
}
