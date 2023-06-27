// **********************************************************
// File: VarianteNormalerAblauf_UnitTest.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;

namespace TestLibrary
{
    [TestFixture]
    public class VarianteNormalerAblauf_UnitTest
    {
        /*
        // Erik Barthelmann, Jonas Hammer, 20.05.2023
        [Test]
        public void NaechsteRolle_Erster_Aufruf_Alice_Erfolg()
        {
            // Arrange 
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);

            // Act
            RolleEnum rolle = normalerAblauf.NaechsteRolle();

            // Assert
            RolleEnum erwarteteRolle = RolleEnum.Alice;
            Assert.AreEqual(erwarteteRolle, rolle);
        }

        [TestCase(2, RolleEnum.Bob)]
        [TestCase(3, RolleEnum.Alice)]
        [TestCase(4, RolleEnum.Bob)]
        [TestCase(5, RolleEnum.Alice)]
        [TestCase(6, RolleEnum.Bob)]
        [TestCase(7, RolleEnum.Alice)]
        public void NaechsteRolle_N_mal_Ausfuehren_Erfolg(int anzahlAusfuehren, RolleEnum erwarteteRolle)
        {
            // Arrange 
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);
            RolleEnum rolle = RolleEnum.Alice;

            // Act
            for (int i = 0; i < anzahlAusfuehren;  i++)
            {
                rolle = normalerAblauf.NaechsteRolle();
            }

            // Assert
            Assert.AreEqual(erwarteteRolle, rolle);
        }

        [Test]
        public void GebeHilfestellung_Phase0_Alice_leicht_Erfolg()
        {
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);

            normalerAblauf.NaechsteRolle();
            List<OperationsEnum> op = normalerAblauf.GebeHilfestellung();

            Assert.Contains(OperationsEnum.textGenerieren, op);
            Assert.Contains(OperationsEnum.zahlGenerieren, op);
            Assert.AreEqual(2, op.Count);
        }

        [Test]
        public void GebeHilfestellung_Phase0_Bob_leicht_Erfolg()
        {
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);

            normalerAblauf.NaechsteRolle();
            List<OperationsEnum> op = normalerAblauf.GebeHilfestellung();

            Assert.AreEqual(0, 0);
        }

        [Test]
        public void GebeHilfestellung_Phase1_Alice_leicht_Erfolg()
        {
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);

            normalerAblauf.NaechsteRolle();
            normalerAblauf.NaechsteRolle();
            normalerAblauf.NaechsteRolle();
            List<OperationsEnum> op = normalerAblauf.GebeHilfestellung();

            Assert.Contains(OperationsEnum.bitfolgeGenerierenAngabe, op);
            Assert.Contains(OperationsEnum.bitfolgeGenerierenZahl, op);
            Assert.Contains(OperationsEnum.polarisationsschemataGenerierenAngabe, op);
            Assert.Contains(OperationsEnum.polarisationsschemataGenerierenZahl, op);
            Assert.Contains(OperationsEnum.photonenGenerieren, op);
            Assert.AreEqual(5, op.Count);
        }

        [Test]
        public void GebeHilfestellung_Phase1_Bob_leicht_Erfolg()
        {
            VarianteNormalerAblauf normalerAblauf = new VarianteNormalerAblauf(0, SchwierigkeitsgradEnum.Leicht);

            normalerAblauf.NaechsteRolle();
            normalerAblauf.NaechsteRolle();
            normalerAblauf.NaechsteRolle();
            normalerAblauf.NaechsteRolle();
            List<OperationsEnum> op = normalerAblauf.GebeHilfestellung();

            // Assert.Contains(OperationsEnum.polarisationsschemataGenerierenAngabe, op);
            // Assert.Contains(OperationsEnum.polarisationsschemataGenerierenZahl, op);
            // Assert.Contains(OperationsEnum.photonenZuBitfolge, op);
            Assert.AreEqual(0, 0);
        }
        */
    }
}
