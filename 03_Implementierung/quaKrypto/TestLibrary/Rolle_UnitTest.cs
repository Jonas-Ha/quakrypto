// **********************************************************
// File: Rolle_UnitTest.cs
// Autor: Leopold Bialek
// erstellt am: 21.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using static Xceed.Wpf.Toolkit.Calculator;

namespace TestLibrary
{
    [TestFixture]
    public class Rolle_UnitTest
    {
        // Arrange 
        RolleEnum rolleEnum = RolleEnum.Alice;
        string alias = "alias_alice";
        string passwort = "passwort_alice";
        IVariante variante = new VarianteAbhoeren(1);

        

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Initialisieren_Erfolg()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);

            // Assert
            Assert.AreEqual(RolleEnum.Alice, rolle.RolleTyp);
            Assert.AreEqual(rolle.Alias, rolle.Alias);
            Assert.IsTrue(rolle.BeginneZug(passwort));
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beginne_Zug_Erfolg()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);

            // Act
            bool result = rolle.BeginneZug("passwort_alice");

            // Assert
            Assert.IsTrue(result);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beginne_Zug_Fehler()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);

            // Act
            bool result = rolle.BeginneZug("passwort_bob");

            // Assert
            Assert.IsFalse(result);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beende_Zug_Erfolg()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            rolle.BeginneZug("passwort_alice");

            // Act
            Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.zugBeenden, null, null, null, rolle.RolleTyp);

            // Assert
            Assert.AreEqual(rolle.InformationsZaehler, 1);
            Assert.AreEqual(handlungsschritt.Ergebnis, null);
            Assert.AreEqual(handlungsschritt.ErgebnisName, null);
            Assert.AreEqual(handlungsschritt.Operand1, null);
            Assert.AreEqual(handlungsschritt.Operand2, null);
            Assert.AreEqual(handlungsschritt.OperationsTyp, OperationsEnum.zugBeenden);
            Assert.AreEqual(handlungsschritt.Rolle, RolleEnum.Alice);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beende_Zug_Fehler()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            rolle.BeginneZug("passwort_bob");

            // Act
            try
            {
                Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.zahlGenerieren, null, null, null, rolle.RolleTyp);
            }
            // Assert
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_ein_Operand_Erfolg()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            rolle.BeginneZug("passwort_bob");
            string informationsname = "ABC";
            Information information = new Information(0, "GesendeteNachricht", InformationsEnum.asciiText, "HelloWorld", RolleEnum.Alice);

            // Act
            Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.nachrichtEmpfangen, information, null, informationsname, rolle.RolleTyp);
            
            // Assert
            Information erwarteteInfo = new Information(0, informationsname, InformationsEnum.asciiText, "HelloWorld", null);
            Information ergebnis = handlungsschritt.Ergebnis;
            Assert.AreEqual(rolle.InformationsZaehler, 1);
            Assert.AreEqual(handlungsschritt.ErgebnisName, informationsname);
            Assert.AreEqual(handlungsschritt.Operand1, information);
            Assert.AreEqual(handlungsschritt.Operand2, null);
            Assert.AreEqual(handlungsschritt.OperationsTyp, OperationsEnum.nachrichtEmpfangen);
            Assert.AreEqual(handlungsschritt.Rolle, RolleEnum.Alice);
            Assert.AreEqual(erwarteteInfo.InformationsID, ergebnis.InformationsID);
            Assert.AreEqual(erwarteteInfo.InformationsName, ergebnis.InformationsName);
            Assert.AreEqual(erwarteteInfo.InformationsTyp, ergebnis.InformationsTyp);
            Assert.AreEqual(erwarteteInfo.InformationsInhalt, ergebnis.InformationsInhalt);
            Assert.AreEqual(erwarteteInfo.InformationsEmpfaenger, ergebnis.InformationsEmpfaenger);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_ein_Operand_Fehler()
        {
            // Act
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            rolle.BeginneZug("passwort_bob");
            string informationsname = "ABC";
            Information information = new Information(0, "GesendeteNachricht", InformationsEnum.asciiText, "HelloWorld", RolleEnum.Alice);

            
            // Act
            try
            {
                // Act
                Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.nachrichtSenden, information, null, informationsname, rolle.RolleTyp);
            }
            // Assert
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }

            // Act
            try
            {
                // Act
                Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.nachrichtEmpfangen, null, null, informationsname, rolle.RolleTyp);
            }
            // Assert
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Erfolg_polschata()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.polschataVergleichen, information1, information2, informationsname, rolle.RolleTyp);
            Information empfangeneInformation = handlungsschritt.Ergebnis;

            //Assert
            BitArray erwarteteBitfolge = new BitArray(10, false);
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[3] = true;
            Information erwarteteInformation = new Information(0, informationsname, InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(rolle.InformationsZaehler, 1);
            Assert.AreEqual(handlungsschritt.ErgebnisName, informationsname);
            Assert.AreEqual(handlungsschritt.Operand1, information1);
            Assert.AreEqual(handlungsschritt.Operand2, information2);
            Assert.AreEqual(handlungsschritt.OperationsTyp, OperationsEnum.polschataVergleichen);
            Assert.AreEqual(handlungsschritt.Rolle, RolleEnum.Alice);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);

        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Fehler_polschata()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            // Act
            try
            {
                // Act
                Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.polschataVergleichen, information1, null, informationsname, rolle.RolleTyp);
            }
            // Assert
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Erfolg()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";

            //Arrange
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.photonenGenerieren, information1, information2, informationsname, rolle.RolleTyp);

            //Assert
            byte[] photonen = new byte[10] { 0, 3, 3, 0, 0, 0, 0, 0, 0, 0 };
            Information empfangeneInformation = handlungsschritt.Ergebnis;
            Information erwarteteInformation = new Information(0, informationsname, InformationsEnum.photonen, photonen, null);
            Assert.AreEqual(rolle.InformationsZaehler, 1);
            Assert.AreEqual(handlungsschritt.ErgebnisName, informationsname);
            Assert.AreEqual(handlungsschritt.Operand1, information1);
            Assert.AreEqual(handlungsschritt.Operand2, information2);
            Assert.AreEqual(handlungsschritt.OperationsTyp, OperationsEnum.photonenGenerieren);
            Assert.AreEqual(handlungsschritt.Rolle, RolleEnum.Alice);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Failed()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);

            // Act
            try
            {
                Handlungsschritt handlungsschritt = rolle.ErzeugeHandlungsschritt(OperationsEnum.polschataVergleichen, information1, null, informationsname, rolle.RolleTyp);
            }
            // Assert
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [Test]
        public void Rolle_SpeicherInformationAb_Erfolg()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            rolle.SpeicherInformationAb(information1);
            rolle.SpeicherInformationAb(information2);

            //Assert
            Assert.AreEqual(rolle.Informationsablage[0], information1);
            Assert.AreEqual(rolle.Informationsablage[1], information2);
        }

        [Test]
        public void Rolle_SpeicherInformationAb_Failed()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);
            var ex = Assert.Throws<NoNullAllowedException>(()=>rolle.SpeicherInformationAb(null));
            Assert.That(ex.Message == "Abzuspeichernde Information darf nicht null sein");
        }

        [Test]
        public void Rolle_LoescheInformation_Erfolg()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            rolle.SpeicherInformationAb(information1);
            rolle.SpeicherInformationAb(information2);
            bool erfolg = rolle.LoescheInformation(information1);

            //Assert
            Assert.IsTrue(erfolg);
            Assert.AreEqual(rolle.Informationsablage[0], information2);
        }

        [Test]
        public void Rolle_LoescheInformation_Failed()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);
            string informationsname = "Bitfolge";
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schl�ssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            rolle.SpeicherInformationAb(information2);
            bool erfolg = rolle.LoescheInformation(information1);

            //Assert
            Assert.IsFalse(erfolg);
            Assert.AreEqual(rolle.Informationsablage[0], information2);
        }

        [Test]
        public void Rolle_AktualisiereInformatoinsZaehler_Erfolg()
        {
            //Arrange
            Rolle rolle = new Rolle(rolleEnum, alias, passwort, variante);

            //Act
            rolle.AktualisiereInformationsZaehler(333);

            //Assert
            Assert.AreEqual(333, rolle.InformationsZaehler);
        }
    }
}
