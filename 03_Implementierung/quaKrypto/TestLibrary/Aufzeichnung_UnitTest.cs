// **********************************************************
// File: Aufzeichnung_UnitTest.cs
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
    public class Aufzeichnung_UnitTest
    {
        // Leopold Bialek, 20.05.2023
        [Test]
        public void Aufzeichnung_HaengeHandlungsschrittAn_Erfolg()
        {
            // Arrange 
            Aufzeichnung aufzeichnung = new Aufzeichnung();
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);
            Handlungsschritt handlungsschritt =
                new Handlungsschritt(OperationsEnum.nachrichtSenden, information, null, RolleEnum.Alice);

            // Act
            aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);

            // Assert
            Assert.AreEqual(aufzeichnung.Handlungsschritte.Last(), handlungsschritt);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Aufzeichnung_HaengeListeHandlungsschritteAn_Erfolg()
        {
            // Arrange 
            Aufzeichnung aufzeichnung = new Aufzeichnung();
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);
            Handlungsschritt handlungsschritt_1 =
                new Handlungsschritt(OperationsEnum.nachrichtSenden, information, null, RolleEnum.Alice);
            Handlungsschritt handlungsschritt_2 =
                new Handlungsschritt(OperationsEnum.nachrichtSenden, information, null, RolleEnum.Alice);
            List<Handlungsschritt> listeHandlungsschritte = new List<Handlungsschritt>();
            listeHandlungsschritte.Add(handlungsschritt_1);
            listeHandlungsschritte.Add(handlungsschritt_2);

            // Act
            aufzeichnung.HaengeListeHandlungsschritteAn(listeHandlungsschritte);

            // Assert
            Assert.AreEqual(aufzeichnung.Handlungsschritte.Last(), handlungsschritt_2);
        }
    }
}
