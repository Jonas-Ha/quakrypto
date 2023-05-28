// **********************************************************
// File: Operationen.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    public class Operationen
    {
        public Information NachrichtSenden()
        {
            return new Information(1, "Name_asciiText", InformationsEnum.asciiText,
                "Inhalt_asciiText", RolleEnum.Bob);
        }

        public Information NachrichtEmpfangen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public  Information NachrichtAbhoeren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        // Rückgabe einer zufälligen Bitfolge aus einer Zufallszahl generiert
        public Information BitfolgeGenerierenZahl(uint informationsID, string informationsname, object informationsInhalt, RolleEnum informationsEmpfaenger)
        {
            var rand = new Random();
            int zufallszahl = rand.Next();

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            BitArray bitArray = new BitArray(zufallszahl, false);

            for (int i = 0; i < zufallszahl; i++)
            {
                // NextDouble gibt einen double s
                if (rand.NextDouble() < 0.5) bitArray[i] = false;
                else bitArray[i] = true;
            }

            return new Information(informationsID, informationsname, InformationsEnum.bitfolge, informationsInhalt, informationsEmpfaenger);
        }

        public Information BitfolgeGenerierenAngabe()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolarisationsschemataGenerierenZahl()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolarisationsschemataGenerierenAngabe()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PhotonenGenerieren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitmaskeGenerieren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolschataVergleichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitfolgenVergleichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PhotonenZuBitfolge()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextVerschluesseln()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextEntschluesseln()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsStreichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsFreiBearbeiten()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information ZugBeenden()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }
    }
}
