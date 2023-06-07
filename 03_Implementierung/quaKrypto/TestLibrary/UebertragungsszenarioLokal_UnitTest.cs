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
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

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
        string aliasEve = "alias_alice";
        string passwortEve = "passwort_alice";
        RolleEnum rolleEnumBob = RolleEnum.Bob;
        string aliasBob = "alias_alice";
        string passwortBob = "passwort_alice";
        RolleEnum rolleEnumAlice2 = RolleEnum.Alice;
        string aliasAlice2 = "alias_alice2";
        string passwortAlice2 = "passwort_alice2";
        
        IVariante variante = new VarianteAbhoeren(1);
        string nameueb = "Ueb";
       
        [Test]
        public void UebungsszenarioLokal_RollenAnlegen_Erfolg()
        {
            //Arrange
            UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.leicht, variante, 1, 4, nameueb);
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
    }
}
