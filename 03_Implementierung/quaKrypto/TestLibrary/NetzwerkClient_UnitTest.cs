//**************************
// File: NetzwerkClient_UnitTest.cs
// Autor: Daniel Hannes
// erstellt am: 05.06.2023
// Projekt: TestLibrary
//**************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [TestFixture]
    public class NetzwerkClient_UnitTest
    {
        #region UDP

        public void BeginneSucheNachLobbys_NichtsMachen_LobbyInformationWirdEmpfangen()
        {
            //Arrange


            //Act


            //Assert

        }

        public void BeendeSucheNachLobbys_NichtsMachen_NichtsPassiert()
        {
            //Arrange


            //Act


            //Assert
        }

        #endregion

        #region TCP
        public void VerbindeMitUebungsszenario_UebungsszenarioNetzwerkBeitrittInfoUebergeben_VerbindungBesteht()
        {
            //Arrange


            //Act


            //Assert
        }

        public void WaehleRolle_RolleUndAliasGewaehlt_NachrichtKommtAn()
        {
            //Arrange


            //Act


            //Assert
        }

        public void GebeRolleFrei_RolleGewaehlt_RolleKommtAn()
        {
            //Arrange


            //Act


            //Assert
        }

        public void BeendeZug_ListeVonHandlungsschrittenAngehaengt_ListeKommtAn()
        {
            //Arrange


            //Act


            //Assert
        }

        public void BeendeUebungsszenario_NichtsMachen_UebungsszenarioEndeKommtAn()
        {
            //Arrange


            //Act


            //Assert
        }
        #endregion
    }
}
