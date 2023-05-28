// **********************************************************
// File: Operationen_UnitTest.cs
// Autor: Jonas Hammer
// erstellt am: 28.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NUnit.Framework;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

namespace TestLibrary
{
    [TestFixture]
    public class Operationen_UnitTest
    {
        // Jonas Hammer, 28.05.2023
        [Test]
        public void Rolle_Beende_Zug_Erfolg()
        {
            // Arrange 
            Operationen operationen = new Operationen();

            // Act
            quaKrypto.Models.Classes.Information information = operationen.BitfolgeGenerierenZahl();

            // Assert
            Assert.IsNotNull(information);
        }
    }
}
