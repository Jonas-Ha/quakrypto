// **********************************************************
// File: UebertragungsszenarioLokal_UnitTest.cs
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
    public class UebertragungsszenarioLokal_UnitTest
    {
        // Leopold Bialek, 20.05.2023
        [Test]
        public void UebertragungsszenarioLokal_Initialisieren_Erfolg()
        {
            // Arrange 
            VarianteNormalerAblauf varianteNormalerAblauf =
                new VarianteNormalerAblauf(1, SchwierigkeitsgradEnum.mittel);

            // Act
            UebungsszenarioLokal uebungsszenarioLokal = new UebungsszenarioLokal(SchwierigkeitsgradEnum.mittel, varianteNormalerAblauf, 1, 2);

            // Assert
            Assert.IsEmpty(uebungsszenarioLokal.Rollen);
            Assert.AreEqual(uebungsszenarioLokal.Schwierigkeitsgrad, SchwierigkeitsgradEnum.mittel);
            Assert.AreEqual(uebungsszenarioLokal.Variante, varianteNormalerAblauf);
            Assert.AreEqual(uebungsszenarioLokal.StartPhase, 1);
            Assert.AreEqual(uebungsszenarioLokal.EndPhase, 2);
            Assert.IsEmpty(uebungsszenarioLokal.Uebertragungskanal.BitKanal);
            Assert.IsEmpty(uebungsszenarioLokal.Uebertragungskanal.PhotonenKanal);
            Assert.IsEmpty(uebungsszenarioLokal.Aufzeichnung.Handlungsschritte);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_VeroeffentlicheLobby_Erfolg()
        {
            //Noch zu ergänzen, sobald Implementierung für VeroeffentlicheLobby erfolgt ist
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void UebertragungsszenarioLokal_RolleHinzufuegen_Erfolg()
        {
            // Arrange 
            VarianteNormalerAblauf varianteNormalerAblauf =
                new VarianteNormalerAblauf(1, SchwierigkeitsgradEnum.mittel);
            UebungsszenarioLokal uebungsszenarioLokal = new UebungsszenarioLokal(SchwierigkeitsgradEnum.mittel, varianteNormalerAblauf, 1, 2);
            Rolle rolle = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");

            // Act
            uebungsszenarioLokal.RolleHinzufuegen(rolle);

            // Assert
            Assert.IsTrue(uebungsszenarioLokal.Rollen.Contains(rolle));
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void UebertragungsszenarioLokal_NaechsterZug_Erfolg()
        {
            // Arrange 
            VarianteNormalerAblauf varianteNormalerAblauf =
                new VarianteNormalerAblauf(1, SchwierigkeitsgradEnum.mittel);
            UebungsszenarioLokal uebungsszenarioLokal = new UebungsszenarioLokal(SchwierigkeitsgradEnum.mittel, varianteNormalerAblauf, 1, 2);
            Rolle alice = new Rolle(RolleEnum.Alice, "alias_alice", "passwort_alice");
            Rolle bob = new Rolle(RolleEnum.Bob, "alias_bob", "passwort_bob");
            uebungsszenarioLokal.RolleHinzufuegen(alice);
            uebungsszenarioLokal.RolleHinzufuegen(bob);

            // Act
            uebungsszenarioLokal.NaechsterZug();

            // Assert
            //Assert.AreEqual(uebungsszenarioLokal.AktuelleRolle, bob);
        }
    }
}
