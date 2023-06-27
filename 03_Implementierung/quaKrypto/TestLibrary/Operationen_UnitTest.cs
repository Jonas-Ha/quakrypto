// **********************************************************
// File: Operationen_UnitTest.cs
// Autor: Jonas Hammer
// erstellt am: 28.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Microsoft.VisualBasic;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using Information = quaKrypto.Models.Classes.Information;

namespace TestLibrary
{
    [TestFixture]
    public class Operationen_UnitTest
    {
        Operationen operationen = new Operationen();
        [Test]
        public void NachrichtSenden_Erfolg()
        {
            //Arrange
            RolleEnum sender = RolleEnum.Alice; 
            bool[] arrpol1 = new bool[10];
            for(int i = 0; i < arrpol1.Length; i++)arrpol1[i] = true;
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Information empfaenger = new Information(0, "Empfaenger", InformationsEnum.zahl, RolleEnum.Bob, null);

            //Act
            Information gesendeteInformation = operationen.NachrichtSenden(2, information, empfaenger, "GesendeteNachricht", RolleEnum.Alice);

            //Assert
            Information erwarteteInformation = new Information(2, "GesendeteNachricht", InformationsEnum.bitfolge, arrpol1, (RolleEnum) empfaenger.InformationsInhalt, sender);
            Assert.AreEqual(erwarteteInformation.InformationsID, gesendeteInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, gesendeteInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, gesendeteInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, gesendeteInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, gesendeteInformation.InformationsEmpfaenger);
            Assert.AreEqual(erwarteteInformation.InformationsSender, gesendeteInformation.InformationsSender);
        }

        [Test]
        public void NachrichtSendenPhotonen_Erfolg()
        {
            //Arrange
            int length = 30;
            byte[] photonen = new byte[length];
            for(int i = 0; i < length; i++) 
            {
                photonen[i] = 0;
            }
            RolleEnum sender = RolleEnum.Alice;
            Information information = new Information(1, "Photonen", InformationsEnum.photonen, photonen, null);
            Information empfaenger = new Information(0, "Empfaenger", InformationsEnum.zahl, RolleEnum.Bob, null);

            //Act
            Information gesendeteInformation = operationen.NachrichtSenden(2, information, empfaenger, "GesendeteNachricht", sender);

            //Assert
            Information erwarteteInformation = new Information(2, "GesendeteNachricht", InformationsEnum.unscharfePhotonen, photonen, (RolleEnum) empfaenger.InformationsInhalt, sender);
            Assert.AreEqual(erwarteteInformation.InformationsID, gesendeteInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, gesendeteInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, gesendeteInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, gesendeteInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, gesendeteInformation.InformationsEmpfaenger);
            Assert.AreEqual(erwarteteInformation.InformationsSender, gesendeteInformation.InformationsSender);
        }

        [Test]
        public void NachrichtSenden_Failed()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            for (int i = 0; i < arrpol1.Length; i++) arrpol1[i] = true;
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            RolleEnum empfaenger = RolleEnum.Bob;

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.NachrichtSenden(2, information, information, "GesendeteNachricht", RolleEnum.Alice));
            
            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ RolleEnum"); 
        }

        [Test]
        public void NachrichtEmpfangen_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            for (int i = 0; i < arrpol1.Length; i++) arrpol1[i] = true;
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.NachrichtEmpfangen(2, information, null, "EmpfangeneNachricht", null);

            //Assert
            Information erwarteteInformation = new Information(2, "EmpfangeneNachricht", InformationsEnum.bitfolge, arrpol1, null);
            Assert.AreEqual(erwarteteInformation.InformationsID,empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgeGenerierenZahl_Erfolg()
        {
            //Arrange
            int length = 10;
            bool[] arrpol1 = new bool[10];
            for (int i = 0; i < arrpol1.Length; i++) arrpol1[i] = true;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge", null);

            //Assert
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(length, ((bool[])empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgeGenerierenZahl_Failed_ZahlKleinerNull()
        {
            //Arrange
            int length = -10;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, null);
            
            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 darf nicht negativ sein");
        }

        [Test]
        public void BitfolgeGenerierenZahl_Failed_KeineZahl()
        {
            //Arrange
            bool[] arr = new bool[10];
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Zahl oder ist kein int");
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Erfolg()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            Information empfangeneInformation = operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwartetarr = new bool[20];
            erwartetarr[1] = true;
            erwartetarr[2] = true;
            erwartetarr[11] = true;
            erwartetarr[12] = true;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwartetarr, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Failed_Keine_Bitfolge()
        {
            //Arrange
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, length, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Failed_Keine_Zahl()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Failed_Zahl_Null()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            int length = 0;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);
            
            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Erfolg()
        {
            //Arrange
            int length = 10;
            bool[] arr = new bool[length];
            for (int i = 0; i < length; i++) arr[i] = true;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge", null);

            //Assert
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.polarisationsschemata, arr, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(length, ((bool[])empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Failed_ZahlKleinerNull()
        {
            //Arrange
            int length = -10;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 darf nicht negativ sein");
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Failed_KeineZahl()
        {
            //Arrange
            bool[] arr = new bool[10];
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Zahl oder ist kein int");
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Erfolg()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            Information empfangeneInformation = operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwartetarr = new bool[20];
            erwartetarr[1] = true;
            erwartetarr[2] = true;
            erwartetarr[11] = true;
            erwartetarr[12] = true;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.polarisationsschemata, erwartetarr, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Failed_Keine_Bitfolge()
        {
            //Arrange
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, length, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Failed_Keine_Zahl()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Failed_Zahl_Null()
        {
            //Arrange
            bool[] arr = new bool[10];
            arr[1] = true;
            arr[2] = true;
            int length = 0;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
        }

        [Test]
        public void PhotonenGenerieren_Erfolg()
        {
            //Arrange
            bool[] arrpol = new bool[10];
            arrpol[1] = true;
            arrpol[2] = true;
            bool[] arrkey = new bool[10];
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge", null);

            //Assert
            byte[] photonen = new byte[10] {0,3,3,0,0,0,0,0,0,0};

            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.photonen, photonen, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PhotonenGenerieren_Failed_Kein_Pol()
        {
            //Arrange
            bool[] arrkey = new bool[10];
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.zahl, 4, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ polarisationsschemata oder ist kein bool[]");
        }

        [Test]
        public void PhotonenGenerieren_Failed_Kein_Key()
        {
            //Arrange
            bool[] arrpol = new bool[10];
            arrpol[1] = true;
            arrpol[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.zahl, 4, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void PhotonenGenerieren_Failed_Ungleiche_Laenge()
        {
            //Arrange
            bool[] arrpol = new bool[10];
            arrpol[1] = true;
            arrpol[2] = true;
            bool[] arrkey = new bool[11];
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "Die Inhalte der beiden Operanden haben nicht die gleiche Länge!");
        }

        [Test]
        public void BitmaskeGenerieren_Erfolg()
        {
            //Arrange
            int maskenLaenge = 10;
            int anzahlEinser = 4;
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.zahl, maskenLaenge, null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitmaske", null);

            //Assert
            Information erwarteteInformation = new Information(2, "Bitmaske", InformationsEnum.bitfolge, new bool[maskenLaenge], null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(maskenLaenge, ((bool[])empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);

            bool[] bits = (bool[])empfangeneInformation.InformationsInhalt;
            int count = 0;
            for(int i  = 0; i < bits.Length; i++)
            {
                if (bits[i]) count++;
            }
            Assert.AreEqual(count, anzahlEinser);
        }

        [Test]
        public void BitmaskeGenerieren_Failed_Keine_Zahl()
        {
            //Arrange
            int maskenLaenge = 10;
            int anzahlEinser = 4;
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.zahl, new bool[2], null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
        }

        [Test]
        public void BitmaskeGenerieren_Failed_Negativ_Zahl()
        {
            //Arrange
            int maskenLaenge = -10;
            int anzahlEinser = 4;
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.zahl, maskenLaenge, null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
        }

        [Test]
        public void BitmaskeGenerieren_Failed_Falscher_Typ()
        {
            //Arrange
            int maskenLaenge = 10;
            int anzahlEinser = 4;
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.bitfolge, maskenLaenge, null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
        }

        [Test]
        public void BitmaskeGenerieren_Failed_anzahlEinser_gr_maskenlaenge()
        {
            //Arrange
            int maskenLaenge = 10;
            int anzahlEinser = 11;
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.zahl, maskenLaenge, null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ Zahl oder ist kein int oder anzahlEinser > LängeBitmaske");
        }

        [Test]
        public void PolschataVergleichen_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            Information empfangeneInformation = operationen.PolschataVergleichen(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwarteteBitfolge = new bool[10];
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[3] = true;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PolschataVergleichen_Failed_KeinBitArray()
        {
            //Arrange
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, 4, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolschataVergleichen(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Polarisationsschemata oder ist kein bool[]");
        }

        [Test]
        public void PolschataVergleichen_Failed_UngleicheLaenge()
        {
            //Arrange
            bool[] arrpol1 = new bool[11];
            arrpol1[1] = true;
            arrpol1[2] = true;
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PolschataVergleichen(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 und operand2 sind nicht gleich lang");
        }

        [Test]
        public void BitfolgenVergleichen_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            Information empfangeneInformation = operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwarteteBitfolge = new bool[10];
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[3] = true;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgenVergleichen_Failed_KeinBitArray()
        {
            //Arrange
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, 4, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
        }

        [Test]
        public void BitfolgenVergleichen_Failed_UngleicheLaenge()
        {
            //Arrange
            bool[] arrpol1 = new bool[11];
            arrpol1[1] = true;
            arrpol1[2] = true;
            bool[] arrpol2 = new bool[10];
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 und operand2 sind nicht gleich lang");
        }

        [Test]
        public void BitfolgeNegieren_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);

            //Act
            Information empfangeneInformation = operationen.BitfolgeNegieren(2, information1, null, "Bitfolge", null);

            //Assert
            bool[] erwarteteBitfolge = new bool[10];
            for(int i = 0; i < erwarteteBitfolge.Length; i++)erwarteteBitfolge[i] = true;
            erwarteteBitfolge[1] = false;
            erwarteteBitfolge[2] = false;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgeNegieren_Failed_KeinBitArray()
        {
            //Arrange
            string keineBitfolge = "Nein";
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, keineBitfolge, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitfolgeNegieren(2, information1, null, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
        }

        [Test]
        public void PhotonenZuBitfolge_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0};
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwarteteBitfolge = new bool[10];
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[2] = false;
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PhotonenZuBitfolge_Erfolg_Random()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 1, 0, 1, 1, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null);

            //Assert
            bool[] erwarteteBitfolge = new bool[10];
            bool[] ausles = (bool[])empfangeneInformation.InformationsInhalt;
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[2] = ausles[2];
            erwarteteBitfolge[5] = ausles[5];
            erwarteteBitfolge[7] = ausles[7];
            erwarteteBitfolge[8] = ausles[8];

            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, erwarteteBitfolge, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_KeinePolschata()
        {
            //Arrange

            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, 4, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray");
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_KeinePhotonen()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, 4, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ unscharfePhotonen oder ist kein byte[]");
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_Ungleiche_Laenge()
        {
            //Arrange
            bool[] arrpol1 = new bool[11];
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 und operand2 sind nicht gleich lang");
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_FalscherTyp()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray");
        }

        [Test]
        public void TextGenerieren_Erfolg()
        {
            //Arrange
            Information text = new Information(0, "Test", InformationsEnum.asciiText, "Hällo", null);
            
            //Act
            Information empfangeneInformation = operationen.TextGenerieren(2, null, text, "Test", null);

            //Assert
            Information erwarteteInformation = new Information(2, "Test", InformationsEnum.asciiText, text.InformationsInhalt, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void TextGenerieren_Failed_Falscher_Typ()
        {
            //Arrange
            Information zahl = new Information(0, "Zahl", InformationsEnum.zahl, 24234234, null);

            var ex = Assert.Throws<Exception>(() => operationen.TextGenerieren(2, null, zahl, "Zahl", null));
            Assert.That(ex.Message == "operand2 ist nicht vom Typ string");
        }

        [Test]
        public void TextLaengeBestimmen_Erfolg()
        {
            //Arrange
            string text = "Hällo";
            int length = 48;
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);

            //Act
            Information empfangeneInformation = operationen.TextLaengeBestimmen(2, information1, null, "zahl", null);

            //Assert
            Information erwarteteInformation = new Information(2, "zahl", InformationsEnum.zahl, length, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void TextLaengeBestimmen_Failed_FalscherTyp()
        {
            //Arrange
            int length = 48;
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, length, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextLaengeBestimmen(2, information1, null, "zahl", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ asciiText oder ist kein string");
        }

        [Test]
        public void TextVerEntschluesseln_Erfolg()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            bool[] schluessel = new bool[length];

            for(int i = 0; i < length; i++) 
            {
                schluessel[i] = (i % 2)==1? true:false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null);
            Information entschluesseltInfo = operationen.TextEntschluesseln(2, empfangeneInformation, information2, "EntschlüsselterText", null);

            //Assert
            Assert.AreEqual(entschluesseltInfo.InformationsTyp, information1.InformationsTyp);
            Assert.AreEqual(entschluesseltInfo.InformationsInhalt, information1.InformationsInhalt);
        }

        [Test]
        public void TextVerschluesseln_Failed_KeinText()
        {
            //Arrange
            int length = 50;
            bool[] schluessel = new bool[length];
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, 4, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ Text oder ist kein String");
        }

        [Test]
        public void TextVerschluesseln_Failed_KeinKey()
        {
            //Arrange
            string text = "Hällo";
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, 4, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void TextVerschluesseln_Failed_KurzerKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 10;
            bool[] schluessel = new bool[length];
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "Der Schlüssel ist zu kurz zum Verschlüsseln");
        }

        [Test]
        public void TextEntschluesseln_Failed_KeinText()
        {
            //Arrange
            int length = 50;
            bool[] schluessel = new bool[length];
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.verschluesselterText, 4, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextEntschluesseln(2, information1, information2, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ VerschlüsselterText oder ist kein String");
        }

        [Test]
        public void TextEntschluesseln_Failed_KeinKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            bool[] schluessel = new bool[length];
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null);
            Information KeinKey = new Information(2, "schluessel", InformationsEnum.bitfolge, 4, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextEntschluesseln(2, empfangeneInformation, KeinKey, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "operand2 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void TextEntschluesseln_Failed_KurzerKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            bool[] schluessel = new bool[length];
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText", null);
            int length2 = 10;
            bool[] schluessel2 = new bool[length2];
            for (int i = 0; i < length2; i++)
            {
                schluessel2[i] = (i % 2) == 1 ? true : false;
            }
            Information KurzerKey = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel2, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.TextEntschluesseln(2, empfangeneInformation, KurzerKey, "VerschlüsselterText", null));

            //Assert
            Assert.That(ex.Message == "Der Schlüssel ist zu kurz zum Entschlüsseln");
        }

        [Test]
        public void BitsStreichen_Erfolg()
        {
            //Arrange
            bool[] zielArray = new bool[10];
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            bool[] zustreichen = new bool[10];
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            Information empfangeneInformation = operationen.BitsStreichen(2, information1, information2, "GekürzteFolge", null);


            //Assert
            bool[] erwartetArr = new bool[8]; //00010101
            erwartetArr[3] = true;
            erwartetArr[5] = true;
            erwartetArr[7] = true;
            Information erwarteteInformation = new Information(2, "GekürzteFolge", InformationsEnum.bitfolge, erwartetArr, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitsStreichen_Failed_keinZielarr()
        {
            //Arrange
            bool[] zustreichen = new bool[10];
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, 4, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitsStreichen(2, information1, information2, "GekürzteFolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void BitsStreichen_Failed_falscherTyp()
        {
            //Arrange
            bool[] zielArray = new bool[10];
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            bool[] zustreichen = new bool[10];
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.photonen, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitsStreichen(2, information1, information2, "GekürzteFolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void BitsStreichen_Failed_ungleicheLaenge()
        {
            //Arrange
            bool[] zielArray = new bool[11];
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            bool[] zustreichen = new bool[10];
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitsStreichen(2, information1, information2, "GekürzteFolge", null));

            //Assert
            Assert.That(ex.Message == "operand1 und operand2 sind nicht gleich lang");
        }

        [Test]
        public void BitsFreiBearbeiten_Erfolg()
        {
            //Arrange
            bool[] zielArray = new bool[10];
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            
            //Act
            Information empfangeneInformation = operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits", null);

            //Assert
            bool[] erwartetArr = new bool[10];
            for (int i = 0; i < 10; i++) erwartetArr[i] = (i % 2) == 1;
            Information erwarteteInformation = new Information(2, "BearbeiteteBits", InformationsEnum.bitfolge, erwartetArr, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitsFreiBearbeiten_Failed_falscherTyp()
        {
            //Arrange
            bool[] zielArray = new bool[10];
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101

            Information information1 = new Information(1, "zielArray", InformationsEnum.asciiText, zielArray, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void BitsFreiBearbeiten_Failed_keinArr()
        {
            //Arrange
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, 4, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits", null));

            //Assert
            Assert.That(ex.Message == "operand1 nicht vom Typ bitfolge oder ist kein bool[]");
        }

        [Test]
        public void ZahlGenerieren_Erfolg()
        {
            //Arrange
            Information zahl = new Information(0, "Zahl", InformationsEnum.zahl, 24234234, null);

            //Act
            Information empfangeneInformation = operationen.ZahlGenerieren(2, null, zahl, "Zahl", null);

            //Assert
            Information erwarteteInformation = new Information(2, "Zahl", InformationsEnum.zahl, zahl.InformationsInhalt, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void ZahlGenerieren_Failed_Falscher_Typ()
        {
            //Arrange
            Information zahl = new Information(0, "Zahl", InformationsEnum.zahl, "string", null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.ZahlGenerieren(2, null, zahl, "Zahl", null));

            //Assert
            Assert.That(ex.Message == "operand2 ist nicht vom Typ int");

        }

        [Test]
        public void ZahlGenerieren_Failed_Falscher_Typ_long()
        {
            //Arrange
            Information zahl = new Information(0, "Zahl", InformationsEnum.zahl, (long) 24234234, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.ZahlGenerieren(2, null, zahl, "Zahl", null));

            //Assert
            Assert.That(ex.Message == "operand2 ist nicht vom Typ int");
        }

        [Test]
        public void InformationUmbenennen_Erfolg()
        {
            //Arrange
            string name1 = "Zahl";
            string name2 = "KeineZahl";
            Information zahl = new Information(0, name1, InformationsEnum.zahl, 24234234, null);
            Information neuerName = new Information(0, "DummyName", InformationsEnum.asciiText, name2, null);

            //Act
            Information empfangeneInformation = operationen.InformationUmbenennen(2, zahl, neuerName, zahl.InformationsName, null);

            //Assert
            Information erwarteteInformation = new Information(2, (string) neuerName.InformationsInhalt, InformationsEnum.zahl, zahl.InformationsInhalt, null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, empfangeneInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void InformationUmbenennen_Failed()
        {
            //Arrange
            string name1 = "Zahl";
            string name2 = "KeineZahl";
            Information zahl = new Information(0, name1, InformationsEnum.zahl, 24234234, null);
            Information neuerName = new Information(0, "DummyName", InformationsEnum.asciiText, 2332, null);

            //Act
            var ex = Assert.Throws<Exception>(() => operationen.InformationUmbenennen(2, zahl, neuerName, zahl.InformationsName, null));

            //Assert
            Assert.That(ex.Message == "operand2 ist nicht vom Typ string");
        }

        [Test]
        public void ZugBeenden_Erfolg()
        {
            //Arrange
            //Act
            Information empfangeneInformation = operationen.ZugBeenden(null, null, null, null, null);

            //Assert
            Assert.AreEqual(empfangeneInformation, null);
        }
    }
}
