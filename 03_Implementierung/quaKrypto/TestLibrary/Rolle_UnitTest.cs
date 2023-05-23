// **********************************************************
// File: Rolle_UnitTest.cs
// Autor: Leopold Bialek
// erstellt am: 21.05.2023
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
    public class Rolle_UnitTest
    {
        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Initialisieren_Erfolg()
        {
            // Arrange 

            // Act
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");

            // Assert
            Assert.AreEqual(RolleEnum.Alice, rolle.RolleTyp);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beginne_Zug_Erfolg()
        {
            // Arrange 
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");

            // Act
            bool result = rolle.BeginneZug("passwort_alice");

            // Assert
            Assert.IsTrue(result);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beginne_Zug_Fehler()
        {
            // Arrange 
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");

            // Act
            bool result = rolle.BeginneZug("passwort_bob");

            // Assert
            Assert.IsFalse(result);
        }

        /*// Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beende_Zug_Erfolg()
        {
            // Arrange 
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");
            rolle.BeginneZug("passwort_alice");

            // Act
            List<Handlungsschritt>? zug = rolle.BeendeZug();

            // Assert
            Assert.IsNotNull(zug);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Beende_Zug_Fehler()
        {
            // Arrange 
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");

            // Act
            List<Handlungsschritt>? zug = rolle.BeendeZug();

            // Assert
            Assert.IsNull(zug);
        }*/

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_ein_Operand_Erfolg()
        {
            //Noch zu ergänzen, sobald Implementierung erfolgt ist
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_ein_Operand_Fehler()
        {
            //Noch zu ergänzen, sobald Implementierung erfolgt ist
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Erfolg()
        {
            //Noch zu ergänzen, sobald Implementierung erfolgt ist
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Rolle_Erzeuge_Handlungsschritt_zwei_Operanden_Fehler()
        {
            //Noch zu ergänzen, sobald Implementierung erfolgt ist
        }
    }
}
