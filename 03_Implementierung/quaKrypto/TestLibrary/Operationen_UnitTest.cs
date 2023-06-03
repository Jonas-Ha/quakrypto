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
            
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, new BitArray(10, true), null);
            RolleEnum empfaenger = RolleEnum.Bob;

            //Act
            Information gesendeteInformation = operationen.NachrichtSenden(2, information, empfaenger, "GesendeteNachricht");

            //Assert
            Information erwarteteInformation = new Information(2, "GesendeteNachricht", InformationsEnum.bitfolge, new BitArray(10, true), empfaenger);
            Assert.AreEqual(erwarteteInformation.InformationsID, gesendeteInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, gesendeteInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, gesendeteInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, gesendeteInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, gesendeteInformation.InformationsEmpfaenger);
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
            
            Information information = new Information(1, "Photonen", InformationsEnum.photonen, photonen, null);
            RolleEnum empfaenger = RolleEnum.Bob;

            //Act
            Information gesendeteInformation = operationen.NachrichtSenden(2, information, empfaenger, "GesendeteNachricht");

            //Assert
            Information erwarteteInformation = new Information(2, "GesendeteNachricht", InformationsEnum.unscharfePhotonen, photonen, empfaenger);
            Assert.AreEqual(erwarteteInformation.InformationsID, gesendeteInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, gesendeteInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, gesendeteInformation.InformationsTyp);
            Assert.AreEqual(erwarteteInformation.InformationsInhalt, gesendeteInformation.InformationsInhalt);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, gesendeteInformation.InformationsEmpfaenger);
        }

        [Test]
        public void NachrichtSenden_Failed()
        {
            //Arrange
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, new BitArray(10, true), null);
            RolleEnum empfaenger = RolleEnum.Bob;

            //Act
            try
            {
                Information gesendeteInformation = operationen.NachrichtSenden(2, information, information, "GesendeteNachricht");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ RolleEnum"));
            }   
        }

        [Test]
        public void NachrichtEmpfangen_Erfolg()
        {
            //Arrange
            Information information = new Information(1, "Bitfolge", InformationsEnum.bitfolge, new BitArray(10, true), RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.NachrichtEmpfangen(2, information, null, "EmpfangeneNachricht");

            //Assert
            Information erwarteteInformation = new Information(2, "EmpfangeneNachricht", InformationsEnum.bitfolge, new BitArray(10, true), null);
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
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge");

            //Assert
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.bitfolge, new BitArray(10, true), null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(length, ((BitArray)empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void BitfolgeGenerierenZahl_Failed_ZahlKleinerNull()
        {
            //Arrange
            int length = -10;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, null);
            
            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 darf nicht negativ sein"));
            }
        }

        [Test]
        public void BitfolgeGenerierenZahl_Failed_KeineZahl()
        {
            //Arrange
            BitArray arr = new BitArray(10);
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgeGenerierenZahl(2, information, null, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Zahl oder ist kein int"));
            }
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Erfolg()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            Information empfangeneInformation = operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwartetarr = new BitArray(20, false);
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
            try
            { 
                Information empfangeneInformation = operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Failed_Keine_Zahl()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0"));
            }
        }

        [Test]
        public void BitfolgeGenerierenAngabe_Failed_Zahl_Null()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            int length = 0;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgeGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0"));
            }
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Erfolg()
        {
            //Arrange
            int length = 10;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, RolleEnum.Bob);

            //Act
            Information empfangeneInformation = operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge");

            //Assert
            Information erwarteteInformation = new Information(2, "Bitfolge", InformationsEnum.polarisationsschemata, new BitArray(10, true), null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(length, ((BitArray)empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Failed_ZahlKleinerNull()
        {
            //Arrange
            int length = -10;
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 darf nicht negativ sein"));
            }
        }

        [Test]
        public void PolarisationsschemataGenerierenZahl_Failed_KeineZahl()
        {
            //Arrange
            BitArray arr = new BitArray(10);
            Information information = new Information(1, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PolarisationsschemataGenerierenZahl(2, information, null, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Zahl oder ist kein int"));
            }
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Erfolg()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            int length = 20;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            Information empfangeneInformation = operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwartetarr = new BitArray(20, false);
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
            try
            {
                Information empfangeneInformation = operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Failed_Keine_Zahl()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, arr, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0"));
            }
        }

        [Test]
        public void PolarisationsschemataGenerierenAngabe_Failed_Zahl_Null()
        {
            //Arrange
            BitArray arr = new BitArray(10, false);
            arr[1] = true;
            arr[2] = true;
            int length = 0;
            Information information1 = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Information information2 = new Information(2, "Zahl", InformationsEnum.zahl, length, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PolarisationsschemataGenerierenAngabe(2, information1, information2, "Bitfolge");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0"));
            }
        }

        [Test]
        public void PhotonenGenerieren_Erfolg()
        {
            //Arrange
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge");

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
            BitArray arrkey = new BitArray(10, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.zahl, 4, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ polarisationsschemata oder ist kein BitArray"));
            }
        }

        [Test]
        public void PhotonenGenerieren_Failed_Kein_Key()
        {
            //Arrange
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.zahl, 4, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void PhotonenGenerieren_Failed_Ungleiche_Laenge()
        {
            //Arrange
            BitArray arrpol = new BitArray(10, false);
            arrpol[1] = true;
            arrpol[2] = true;
            BitArray arrkey = new BitArray(11, false);
            arrkey[1] = true;
            arrkey[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol, null);
            Information information2 = new Information(2, "Schlüssel", InformationsEnum.bitfolge, arrkey, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("Die Inhalte der beiden Operanden haben nicht die gleiche Länge!"));
            }
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
            Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitmaske");

            //Assert
            Information erwarteteInformation = new Information(2, "Bitmaske", InformationsEnum.bitfolge, new BitArray(maskenLaenge, false), null);
            Assert.AreEqual(erwarteteInformation.InformationsID, empfangeneInformation.InformationsID);
            Assert.AreEqual(erwarteteInformation.InformationsName, empfangeneInformation.InformationsName);
            Assert.AreEqual(erwarteteInformation.InformationsTyp, empfangeneInformation.InformationsTyp);
            Assert.AreEqual(maskenLaenge, ((BitArray)empfangeneInformation.InformationsInhalt).Length);
            Assert.AreEqual(erwarteteInformation.InformationsEmpfaenger, empfangeneInformation.InformationsEmpfaenger);

            BitArray bits = (BitArray)empfangeneInformation.InformationsInhalt;
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
            Information information1 = new Information(1, "maskenLaenge", InformationsEnum.zahl, new BitArray(2,false), null);
            Information information2 = new Information(2, "anzahlEinser", InformationsEnum.zahl, anzahlEinser, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null"));
            }
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
            try
            {
                Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null"));
            }
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
            try
            {
                Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null"));
            }
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
            try
            {
                Information empfangeneInformation = operationen.BitmaskeGenerieren(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ Zahl oder ist kein int oder anzahlEinser > LängeBitmaske"));
            }
        }

        [Test]
        public void PolschataVergleichen_Erfolg()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            Information empfangeneInformation = operationen.PolschataVergleichen(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwarteteBitfolge = new BitArray(10, false);
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
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, 4, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PolschataVergleichen(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Polarisationsschemata oder ist kein Bitarray"));
            }
        }

        [Test]
        public void PolschataVergleichen_Failed_UngleicheLaenge()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(11, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol2, null);
            
            //Act
            try
            {
                Information empfangeneInformation = operationen.PolschataVergleichen(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 und operand2 sind nicht gleich lang"));
            }
        }

        [Test]
        public void BitfolgenVergleichen_Erfolg()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            Information empfangeneInformation = operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwarteteBitfolge = new BitArray(10, false);
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
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, 4, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Bitfolge oder ist kein Bitarray"));
            }
        }

        [Test]
        public void BitfolgenVergleichen_Failed_UngleicheLaenge()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(11, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            BitArray arrpol2 = new BitArray(10, false);
            arrpol2[2] = true;
            arrpol2[3] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "Bitfolge", InformationsEnum.bitfolge, arrpol2, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitfolgenVergleichen(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 und operand2 sind nicht gleich lang"));
            }
        }

        [Test]
        public void PhotonenZuBitfolge_Erfolg()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0};
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwarteteBitfolge = new BitArray(10, false);
            erwarteteBitfolge[1] = true;
            erwarteteBitfolge[2] = true;
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
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 1, 0, 1, 1, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");

            //Assert
            BitArray erwarteteBitfolge = new BitArray(10, false);
            BitArray ausles = (BitArray)empfangeneInformation.InformationsInhalt;
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
            try
            {
                Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray"));
            }
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_KeinePhotonen()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, 4, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ unscharfePhotonen oder ist kein byte[]"));
            }
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_Ungleiche_Laenge()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(11, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.polarisationsschemata, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 und operand2 sind nicht gleich lang"));
            }
        }

        [Test]
        public void PhotonenZuBitfolge_Failed_FalscherTyp()
        {
            //Arrange
            BitArray arrpol1 = new BitArray(10, false);
            arrpol1[1] = true;
            arrpol1[2] = true;
            byte[] Photonen = new byte[10] { 0, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
            Information information1 = new Information(1, "Polarisationsschemata", InformationsEnum.bitfolge, arrpol1, null);
            Information information2 = new Information(2, "unscharfePhotonen", InformationsEnum.unscharfePhotonen, Photonen, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.PhotonenZuBitfolge(2, information1, information2, "Bitfolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray"));
            }
        }

        [Test]
        public void TextVerEntschluesseln_Erfolg()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            BitArray schluessel = new BitArray(length, false);
            for(int i = 0; i < length; i++) 
            {
                schluessel[i] = (i % 2)==1? true:false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            Information entschluesseltInfo = operationen.TextEntschluesseln(2, empfangeneInformation, information2, "EntschlüsselterText");

            //Assert
            Assert.AreEqual(entschluesseltInfo.InformationsTyp, information1.InformationsTyp);
            Assert.AreEqual(entschluesseltInfo.InformationsInhalt, information1.InformationsInhalt);
        }

        [Test]
        public void TextVerschluesseln_Failed_KeinText()
        {
            //Arrange
            int length = 50;
            BitArray schluessel = new BitArray(length, false);
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, 4, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ Text oder ist kein String"));
            }
        }

        [Test]
        public void TextVerschluesseln_Failed_KeinKey()
        {
            //Arrange
            string text = "Hällo";
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, 4, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void TextVerschluesseln_Failed_KurzerKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 10;
            BitArray schluessel = new BitArray(length, false);
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("Der Schlüssel ist zu kurz zum Verschlüsseln"));
            }
        }

        [Test]
        public void TextEntschluesseln_Failed_KeinText()
        {
            //Arrange
            int length = 50;
            BitArray schluessel = new BitArray(length, false);
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.verschluesselterText, 4, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.TextEntschluesseln(2, information1, information2, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ VerschlüsselterText oder ist kein String"));
            }
        }

        [Test]
        public void TextEntschluesseln_Failed_KeinKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            BitArray schluessel = new BitArray(length, false);
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            Information KeinKey = new Information(2, "schluessel", InformationsEnum.bitfolge, 4, null);

            //Act
            try
            {
                Information entschluesselterInfo = operationen.TextEntschluesseln(2, empfangeneInformation, KeinKey, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand2 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void TextEntschluesseln_Failed_KurzerKey()
        {
            //Arrange
            string text = "Hällo";
            int length = 50;
            BitArray schluessel = new BitArray(length, false);
            for (int i = 0; i < length; i++)
            {
                schluessel[i] = (i % 2) == 1 ? true : false;
            }
            Information information1 = new Information(1, "Text", InformationsEnum.asciiText, text, null);
            Information information2 = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel, null);
            Information empfangeneInformation = operationen.TextVerschluesseln(2, information1, information2, "VerschlüsselterText");
            int length2 = 10;
            BitArray schluessel2 = new BitArray(length, false);
            for (int i = 0; i < length2; i++)
            {
                schluessel2[i] = (i % 2) == 1 ? true : false;
            }
            Information KurzerKey = new Information(2, "schluessel", InformationsEnum.bitfolge, schluessel2, null);

            //Act
            try
            {
                Information entschluesselterInfo = operationen.TextEntschluesseln(2, empfangeneInformation, KurzerKey, "VerschlüsselterText");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("Der Schlüssel ist zu kurz zum Entschlüsseln"));
            }
        }

        [Test]
        public void BitsStreichen_Erfolg()
        {
            //Arrange
            BitArray zielArray = new BitArray(10, false);
            for(int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            BitArray zustreichen = new BitArray(10, false);
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            Information empfangeneInformation = operationen.BitsStreichen(2, information1, information2, "GekürzteFolge");


            //Assert
            BitArray erwartetArr = new BitArray(8, false); //00010101
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
            BitArray zustreichen = new BitArray(10, false);
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, 4, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitsStreichen(2, information1, information2, "GekürzteFolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void BitsStreichen_Failed_falscherTyp()
        {
            //Arrange
            BitArray zielArray = new BitArray(10, false);
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            BitArray zustreichen = new BitArray(10, false);
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.photonen, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitsStreichen(2, information1, information2, "GekürzteFolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void BitsStreichen_Failed_ungleicheLaenge()
        {
            //Arrange
            BitArray zielArray = new BitArray(11, false);
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            BitArray zustreichen = new BitArray(10, false);
            zustreichen[1] = true;
            zustreichen[3] = true;
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            Information information2 = new Information(2, "zustreichen", InformationsEnum.bitfolge, zustreichen, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitsStreichen(2, information1, information2, "GekürzteFolge");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 und operand2 sind nicht gleich lang"));
            }
        }

        [Test]
        public void BitsFreiBearbeiten_Erfolg()
        {
            //Arrange
            BitArray zielArray = new BitArray(10, false);
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101
            
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, zielArray, null);
            
            //Act
            Information empfangeneInformation = operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits");

            //Assert
            BitArray erwartetArr = new BitArray(10, false);
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
            BitArray zielArray = new BitArray(10, false);
            for (int i = 0; i < 10; i++) zielArray[i] = (i % 2) == 1; //0101010101

            Information information1 = new Information(1, "zielArray", InformationsEnum.asciiText, zielArray, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void BitsFreiBearbeiten_Failed_keinArr()
        {
            //Arrange
            Information information1 = new Information(1, "zielArray", InformationsEnum.bitfolge, 4, null);

            //Act
            try
            {
                Information empfangeneInformation = operationen.BitsFreiBearbeiten(2, information1, null, "BearbeiteteBits");
            }
            //Assert
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Equals("operand1 nicht vom Typ bitfolge oder ist kein BitArray"));
            }
        }

        [Test]
        public void ZugBeenden_Erfolg()
        {
            //Arrange
            //Act
            Information empfangeneInformation = operationen.ZugBeenden(null, null, null, null);

            //Assert
            Assert.AreEqual(empfangeneInformation, null);
        }

        // Jonas Hammer, 28.05.2023
        [Test]
        public void Rolle_Beende_Zug_Erfolg()
        {
            // Arrange 
       
            
            // Act
            //quaKrypto.Models.Classes.Information information = operationen.BitfolgeGenerierenZahl(0, "Bitfolge", null, RolleEnum.Alice);

            // Assert
            //Assert.IsNotNull(information);
        }
    }
}
