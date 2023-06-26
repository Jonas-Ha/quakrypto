// **********************************************************
// File: UebertragungsszenarioLokal_UnitTest.cs
// Autor: Leopold Bialek
// erstellt am: 21.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using static Xceed.Wpf.Toolkit.Calculator;
using Information = quaKrypto.Models.Classes.Information;

namespace TestLibrary
{
    [TestFixture]
    public class UebertragungsszenarioLokal_UnitTest
    {
        // Arrange 
        RolleEnum rolleEnum = RolleEnum.Alice;
        string alias = "alias_alice";
        string passwort = "passwort_alice";
        RolleEnum rolleEnumEve = RolleEnum.Eve;
        string aliasEve = "alias_eve";
        string passwortEve = "passwort_eve";
        RolleEnum rolleEnumBob = RolleEnum.Bob;
        string aliasBob = "alias_bob";
        string passwortBob = "passwort_bob";
        RolleEnum rolleEnumAlice2 = RolleEnum.Alice;
        string aliasAlice2 = "alias_alice2";
        string passwortAlice2 = "passwort_alice2";
        

        string nameueb = "Ueb";
       
        [Test]
        public void UebungsszenarioLokal_RollenAnlegen_Erfolg()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 1, 4, nameueb);

            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);
            Ueb.RolleHinzufuegen(rolle3);

            //Assert
            List<Rolle> expected = new List<Rolle>{ rolle, rolle2, rolle3 };
            Assert.AreEqual(expected[0], Ueb.Rollen[0]);
            Assert.AreEqual(expected[1], Ueb.Rollen[1]);
            Assert.AreEqual(expected[2], Ueb.Rollen[2]);
        }

        [Test]
        public void UebungsszenarioLokal_RollenAnlegen_Failed()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 1, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumAlice2, aliasAlice2, passwortAlice2);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);
            bool erfolg = Ueb.RolleHinzufuegen(rolle3);

            //Assert
            List<Rolle> expected = new List<Rolle> { rolle, rolle2, rolle3 };
            Assert.AreEqual(expected[0], Ueb.Rollen[0]);
            Assert.AreEqual(expected[1], Ueb.Rollen[1]);
            Assert.IsFalse(erfolg);
            
            //Assert
            Assert.That(()=> Assert.AreEqual(expected[2], Ueb.Rollen[2]), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());    
        }

        [Test]
        public void UebungsszenarioLokal_GebeRolleFrei_Erfolg()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 1, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);
            Ueb.RolleHinzufuegen(rolle3);

            //Act
            Ueb.GebeRolleFrei(rolleEnumEve);

            //Assert
            List<Rolle> expected = new List<Rolle> { rolle, rolle3 };
            Assert.AreEqual(expected.Count, Ueb.Rollen.Count);
            Assert.AreEqual(expected[0], Ueb.Rollen[0]);
            Assert.AreEqual(expected[1], Ueb.Rollen[1]);
        }

        [Test]
        public void UebungsszenarioLokal_Starten_Erfolg()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 1, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);
            Ueb.RolleHinzufuegen(rolle3);
            
            Assert.IsTrue(Ueb.Starten());
            Assert.AreEqual(rolle, Ueb.AktuelleRolle);
        }

        [Test]
        public void UebungsszenarioLokal_Starten_Failed_ZuwenigeRollen()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 0, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);

            Assert.IsFalse(Ueb.Starten());
        }

        [Test]
        public void UebungsszenarioLokal_Starten_Failed_FalscheRollen()
        {
            //Arrange
            IVariante varianteNorm = new VarianteNormalerAblauf(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteNorm, 1, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle2);

            Assert.IsFalse(Ueb.Starten());
        }

        [Test]
        public void UebungsszenarioLokal_NaechsterZug_Erfolg()
        {
            //Arrange
            IVariante varianteNorm = new VarianteNormalerAblauf(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteNorm, 1, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle3);

            Assert.IsTrue(Ueb.Starten());
            Assert.AreEqual(rolle, Ueb.AktuelleRolle);
            Assert.IsTrue(Ueb.AktuelleRolle.BeginneZug(passwort));

            Information text = new Information(0, "text", InformationsEnum.asciiText, "HelloWorld", null);
            Information laenge = new Information(0, "laenge", InformationsEnum.zahl, 80, null);

            Information erg1 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, text, "NameDerInfo", Ueb.AktuelleRolle.RolleTyp);
            Information erg2 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textLaengeBestimmen, erg1, null, "TextLaenge", Ueb.AktuelleRolle.RolleTyp);
            Assert.AreEqual(text.InformationsInhalt, erg1.InformationsInhalt);
            Assert.AreEqual(laenge.InformationsInhalt, erg2.InformationsInhalt);

            Ueb.NaechsterZug();
            Assert.IsTrue(Ueb.GebeBildschirmFrei(passwortBob));
            Assert.AreEqual(rolle3, Ueb.AktuelleRolle);

            Handlungsschritt hand1 = new Handlungsschritt(0, OperationsEnum.textGenerieren, null, text, "NameDerInfo", RolleEnum.Alice);
            Handlungsschritt hand2 = new Handlungsschritt(1, OperationsEnum.textLaengeBestimmen, erg1, null, "TextLaenge", RolleEnum.Alice);
            
            Assert.AreEqual(hand1.Ergebnis.InformationsID, Ueb.Aufzeichnung.Handlungsschritte[0].Ergebnis.InformationsID);
            Assert.AreEqual(hand1.Ergebnis.InformationsInhalt, Ueb.Aufzeichnung.Handlungsschritte[0].Ergebnis.InformationsInhalt);
            Assert.AreEqual(hand2.Ergebnis.InformationsID, Ueb.Aufzeichnung.Handlungsschritte[1].Ergebnis.InformationsID);
            Assert.AreEqual(hand2.Ergebnis.InformationsInhalt, Ueb.Aufzeichnung.Handlungsschritte[1].Ergebnis.InformationsInhalt);
            
            Information erg3 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, text, "NameDerInfo", Ueb.AktuelleRolle.RolleTyp);
            Handlungsschritt hand3 = new Handlungsschritt(2, OperationsEnum.textGenerieren, null, text, "NameDerInfo", RolleEnum.Bob);
            
            Assert.AreEqual(hand3.Ergebnis.InformationsID, Ueb.Aufzeichnung.Handlungsschritte[2].Ergebnis.InformationsID);
            Assert.AreEqual(hand3.Ergebnis.InformationsInhalt, Ueb.Aufzeichnung.Handlungsschritte[2].Ergebnis.InformationsInhalt);
        }
        
        [Test]
        public void UebungsszenarioLokal_GebeBildschrimFrei_Failed_FalscheRollen()
        {
            //Arrange
            IVariante varianteAb = new VarianteAbhoeren(1);
            UebungsszenarioLokal Ueb2 = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteAb, 0, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb2.RolleHinzufuegen(rolle);
            Ueb2.RolleHinzufuegen(rolle2);
            Ueb2.RolleHinzufuegen(rolle3);

            Assert.IsTrue(Ueb2.Starten());
            Assert.IsFalse(Ueb2.GebeBildschirmFrei(passwortBob));
        }

        [Test]
        public void UebungsszenarioLokal_SpeichereInformationAb_Erfolg()
        {
            //Arrange
            IVariante varianteNorm = new VarianteNormalerAblauf(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteNorm, 0, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle3);

            Assert.IsTrue(Ueb.Starten());
            Assert.AreEqual(rolle, Ueb.AktuelleRolle);
            Assert.IsTrue(Ueb.AktuelleRolle.BeginneZug(passwort));

            Information text = new Information(0, "text", InformationsEnum.asciiText, "HelloWorld", null);
            Information laenge = new Information(0, "laenge", InformationsEnum.zahl, 80, null);

            Information erg1 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, text, "NameDerInfo", Ueb.AktuelleRolle.RolleTyp);
            Information erg2 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textLaengeBestimmen, erg1, null, "TextLaenge", Ueb.AktuelleRolle.RolleTyp);

            Ueb.SpeichereInformationenAb(erg1);
            Ueb.SpeichereInformationenAb(erg2);

            Assert.AreEqual(erg1, Ueb.AktuelleRolle.Informationsablage[0]);
            Assert.AreEqual(erg2, Ueb.AktuelleRolle.Informationsablage[1]);
        }


        [Test]
        public void UebungsszenarioLokal_LoescheInformationAb_Erfolg()
        {
            //Arrange
            IVariante varianteNorm = new VarianteNormalerAblauf(1);
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteNorm, 0, 4, nameueb);
            Rolle rolle = new Rolle(rolleEnum, alias, passwort);
            Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

            Ueb.RolleHinzufuegen(rolle);
            Ueb.RolleHinzufuegen(rolle3);

            Assert.IsTrue(Ueb.Starten());
            Assert.AreEqual(rolle, Ueb.AktuelleRolle);
            Assert.IsTrue(Ueb.AktuelleRolle.BeginneZug(passwort));

            Information text = new Information(0, "text", InformationsEnum.asciiText, "HelloWorld", null);
            Information laenge = new Information(0, "laenge", InformationsEnum.zahl, 80, null);

            Information erg1 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, text, "NameDerInfo", Ueb.AktuelleRolle.RolleTyp);
            Information erg2 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textLaengeBestimmen, erg1, null, "TextLaenge", Ueb.AktuelleRolle.RolleTyp);

            Ueb.SpeichereInformationenAb(erg1);
            Ueb.SpeichereInformationenAb(erg2);

            Assert.AreEqual(erg1, Ueb.AktuelleRolle.Informationsablage[0]);
            Assert.AreEqual(erg2, Ueb.AktuelleRolle.Informationsablage[1]);

            Ueb.LoescheInformation(erg1.InformationsID);
            Assert.AreEqual(erg2, Ueb.AktuelleRolle.Informationsablage[0]);
        }
    }
}
