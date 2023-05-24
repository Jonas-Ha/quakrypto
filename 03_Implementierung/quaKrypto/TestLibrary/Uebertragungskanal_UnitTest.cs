// **********************************************************
// File: Uebertragungskanal_UnitTest.cs
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
    public class Uebertragungskanal_UnitTest
    {/*
        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_SpeichereNachricht_BitKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);

            // Act
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Assert
            Assert.AreEqual(uebertragungskanal.BitKanal.Last(), information);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_SpeichereNachricht_PhotonenKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_unscharfePhotonen", InformationsEnum.unscharfePhotonen,
                "Inhalt_unscharfePhotonen", RolleEnum.Bob);

            // Act
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Assert
            Assert.AreEqual(uebertragungskanal.PhotonenKanal.Last(), information);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_LeseKanalAus_BitKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Act
            List<Information>? bitKanal = uebertragungskanal.LeseKanalAus(KanalEnum.bitKanal);

            // Assert
            Assert.AreEqual(bitKanal.Last(), information);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_LeseKanalAus_PhotonenKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_unscharfePhotonen", InformationsEnum.unscharfePhotonen,
                "Inhalt_unscharfePhotonen", RolleEnum.Bob);
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Act
            List<Information>? photonenKanal = uebertragungskanal.LeseKanalAus(KanalEnum.photonenKanal);

            // Assert
            Assert.AreEqual(photonenKanal.Last(), information);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_LoescheNachricht_BitKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Act
            uebertragungskanal.LoescheNachricht(KanalEnum.bitKanal, 1);
            List<Information>? bitKanal = uebertragungskanal.LeseKanalAus(KanalEnum.bitKanal);

            // Assert
            Assert.IsEmpty(bitKanal);
        }

        // Leopold Bialek, 20.05.2023
        [Test]
        public void Uebertragungskanal_LoescheNachricht_PhotonenKanal_Erfolg()
        {
            // Arrange 
            Uebertragungskanal uebertragungskanal = new Uebertragungskanal();
            Information information = new Information(1, "Name_unscharfePhotonen", InformationsEnum.unscharfePhotonen,
                "Inhalt_unscharfePhotonen", RolleEnum.Bob);
            uebertragungskanal.SpeicherNachrichtAb(information);

            // Act
            uebertragungskanal.LoescheNachricht(KanalEnum.photonenKanal, 1);
            List<Information>? photonenKanal = uebertragungskanal.LeseKanalAus(KanalEnum.photonenKanal);

            // Assert
            Assert.IsEmpty(photonenKanal);
        }
    */}
}
