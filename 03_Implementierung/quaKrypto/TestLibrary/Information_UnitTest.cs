using NUnit.Framework;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [TestFixture]
    public class Information_UnitTest
    {
        [Test]
        public void Information_Null_Erfolg()
        {
            //Arrange
            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.photonen, null, null);

            //Act
            string erg = ergInformation.InformationsInhaltToString;

            //Assert
            Assert.IsTrue(erg == string.Empty);
        }

        [Test]
        public void Information_BitArray_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.bitfolge, arrpol1, null);

            //Act
            string erg = information1.InformationsInhaltToString;

            //Assert
            string erwartet = "0110000000";
            Assert.IsTrue(erg == erwartet);
        }

        [Test]
        public void Information_PhotonenToString_Erfolg()
        {
            //Arrange
            byte[] photonen = new byte[10] { 0, 3, 3, 1, 2, 0, 0, 0, 0, 0 };

            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.photonen, photonen, null);

            //Act
            string erg = ergInformation.InformationsInhaltToString;

            //Assert
            string erwartet = "╲──│╱╲╲╲╲╲";
            Assert.IsTrue(erg == erwartet);
        }

        [Test]
        public void Information_Polschata_Erfolg()
        {
            //Arrange
            bool[] arrpol1 = new bool[10];
            arrpol1[1] = true;
            arrpol1[2] = true;
            Information information1 = new Information(1, "Bitfolge", InformationsEnum.polarisationsschemata, arrpol1, null);

            //Act
            string erg = information1.InformationsInhaltToString;

            //Assert
            string erwartet = "✕✛✛✕✕✕✕✕✕✕";
            Assert.IsTrue(erg == erwartet);
        }

        [Test]
        public void Information_unscharfePhotonenToString_Erfolg()
        {
            //Arrange
            byte[] photonen = new byte[10] { 0, 3, 3, 1, 2, 0, 0, 0, 0, 0 };

            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.unscharfePhotonen, photonen, null);

            //Act
            string erg = ergInformation.InformationsInhaltToString;

            //Assert
            string erwartet = "**********";
            Assert.IsTrue(erg == erwartet);
        }

        [Test]
        public void Information_asciiTextToString_Erfolg()
        {
            //Arrange
            string text = "Hello";

            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.asciiText, text, null);

            //Act
            string erg = ergInformation.InformationsInhaltToString;

            //Assert
            string erwartet = "Hello";
            Assert.IsTrue(erg == erwartet);
        }

        [Test]
        public void Information_verschlüsselterTextToString_Erfolg()
        {
            //Arrange
            string text = "Hello";

            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.verschluesselterText, text, null);

            //Act
            string erg = ergInformation.InformationsInhaltToString;

            //Assert
            string erwartet = "Hello";
            Assert.IsTrue(erg == erwartet);
        }
    }
}
