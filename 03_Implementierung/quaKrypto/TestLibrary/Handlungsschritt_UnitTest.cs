// **********************************************************
// File: Handlungsschritt_UnitTest.cs
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
using Moq;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;

namespace TestLibrary
{
    [TestFixture]
    public class Handlungsschritt_UnitTest
    {
        // Leopold Bialek, 20.05.2023
        [Test]
        public void Handlungsschritt_Initialisieren_Erfolg()
        {
            // Arrange 
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);


            // Act
            Handlungsschritt handlungsschritt =
                new Handlungsschritt(information.InformationsID, OperationsEnum.nachrichtSenden, information, null, information.InformationsName,RolleEnum.Alice);

            // Assert
            Assert.AreEqual(handlungsschritt.OperationsTyp, OperationsEnum.nachrichtSenden);
            Assert.AreEqual(handlungsschritt.Operand1, information);
            Assert.AreEqual(handlungsschritt.Operand2, null);
            Assert.AreEqual(handlungsschritt.Rolle, RolleEnum.Alice);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Handlungsschritt_HandlungsschrittAusfuehren_Erfolg()
        {
            //Noch zu ergänzen, sobald Implementierung für Operationen er erfolgt ist
        }
    }
}
