//**************************
// File: RechnerUnitTest.cs
// Autor: TestTeam
// erstellt am: 06.05.2023
// Projekt: TestLibrary
//**************************

using NUnit.Framework;
using quaKrypto.Models;
using System;

namespace TestLibrary
{
    [TestFixture]
    public class RechnerUnitTest
    {
        // Alexander Denner, 06.05.2023
        [Test]
        public void Addieren_ZweiZahlen_KorrektesErgebnis()
        {
            //Arrange 
            Rechner rechner = new Rechner();
            const int zahl1 = 1;
            const int zahl2 = 2;

            //Act
            int ergebnis = rechner.Addieren(zahl1, zahl2);

            //Assert
            const int erwartetesErgebnis = 3;
            Assert.AreEqual(erwartetesErgebnis, ergebnis);
        }

        //Erik Barthelmann, 07.05.2023
        [Test]
        public void Addieren_MaxInteger_Overflow()
        {
            //Arrange
            Rechner rechner = new Rechner();
            const int zahl1 = 1;

            //Act
            void AddierenMitMaxInteger() => rechner.Addieren(zahl1, int.MaxValue);


            //Assert
            Assert.Throws<OverflowException>(AddierenMitMaxInteger);
        }

        //Erik Barthelmann, 07.05.2023
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Addieren_Null_SelbeZahl(int zahl)
        {
            //Arrange
            Rechner rechner = new Rechner();
            const int zahlNull = 0;

            //Act
            int ergebnis = rechner.Addieren(zahl, zahlNull);

            //Assert
            int erwartetesErgebnis = zahl;
            Assert.AreEqual(erwartetesErgebnis, ergebnis);

        }


    }
}