﻿// **********************************************************
// File: Operationen.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections;
using System.Text;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    public class Operationen
    {
        private Random random;

        public Operationen(int seed = -1)
        {
            random = new Random(seed == -1 ? (int)DateTime.Now.Ticks : seed);
        }

        // operand1 gibt die zu sendende Information an
        // operand2 gibt den Empfänger der Information an
        public Information NachrichtSenden(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null || sender == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand2.InformationsInhalt.GetType().Equals(typeof(RolleEnum))))
            {
                throw new Exception("operand2 nicht vom Typ RolleEnum");
            }

            RolleEnum empfaenger = (RolleEnum) operand2.InformationsInhalt;
            
            if(operand1.InformationsTyp == InformationsEnum.photonen)
            {
                return new Information(informationsID, ergebnisName, InformationsEnum.unscharfePhotonen, operand1.InformationsInhalt, empfaenger, sender);
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, empfaenger, sender);
        }

        // operand1 enthält die zu empfangende Information
        // operand2 null
        public Information NachrichtEmpfangen(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            return new Information(operand1.InformationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        // operand1 null
        // operand2 null
        // Hinweis: Wird wahrscheinlich nicht mehr verwendet!
        public  Information NachrichtAbhoeren(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        // operand1 Länge der zu generierenden Bitfolge
        // operand2 null
        // Hinweis: Rückgabe einer zufälligen Bitfolge aus einer Zufallszahl generiert
        public Information BitfolgeGenerierenZahl(int informationsID, Information operand1, Information? operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            int zufallszahl = (int)operand1.InformationsInhalt;

            if(zufallszahl<=0)
            {
                throw new Exception("operand1 darf nicht negativ sein");
            }

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            bool[] bitArray = new bool[zufallszahl];

            for (int i = 0; i < zufallszahl; i++)
            {
                if (random.NextDouble() < 0.5) bitArray[i] = false;
                else bitArray[i] = true;
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        // operand1 eingegebene Bitfolge
        // operand2 Länge der zu generierenden Bitfolge
        public Information BitfolgeGenerierenAngabe(int informationsID,  Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(int)))) || (((int)operand2.InformationsInhalt <= 0 )))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
            }

            int arrayLaenge = (int)operand2.InformationsInhalt;

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            bool[] bitArray = new bool[arrayLaenge];
            bool[] bitArrayEingegeben = (bool[])operand1.InformationsInhalt;
            int bitArrayEingegebenLaenge = bitArrayEingegeben.Length;

            for (int i = 0; i < arrayLaenge; i++)
            {
                bitArray[i] = bitArrayEingegeben[i%bitArrayEingegebenLaenge];
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        // operand1 Länge der zu generierenden Polarisationsschemata
        // operand2 null
        public Information PolarisationsschemataGenerierenZahl(int informationsID, Information operand1, Information? operand2, string ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            int zufallszahl = (int)operand1.InformationsInhalt;

            if (zufallszahl <= 0)
            {
                throw new Exception("operand1 darf nicht negativ sein");
            }

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            bool[] bitArray = new bool[zufallszahl];

            for (int i = 0; i < zufallszahl; i++)
            {
                if (random.NextDouble() < 0.5) bitArray[i] = false;
                else bitArray[i] = true;
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.polarisationsschemata, bitArray, null);
        }

        // operand1 eingegebene Polarisationsschemata
        // operand2 Länge der zu generierenden Polschata
        public Information PolarisationsschemataGenerierenAngabe(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(int)))) || (((int)operand2.InformationsInhalt <= 0)))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
            }

            int arrayLaenge = (int)operand2.InformationsInhalt;

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            bool[] bitArray = new bool[arrayLaenge];
            bool[] bitArrayEingegeben = (bool[])operand1.InformationsInhalt;
            int bitArrayEingegebenLaenge = bitArrayEingegeben.Length;

            for (int i = 0; i < arrayLaenge; i++)
            {
                bitArray[i] = bitArrayEingegeben[i % bitArrayEingegebenLaenge];
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.polarisationsschemata, bitArray, null);
        }

        // operand1 Polarisationsschemata
        // operand2 Schlüssel bestehend aus Bits
        public Information PhotonenGenerieren(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ polarisationsschemata oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            bool[] op1Inhalt = (bool[])operand1.InformationsInhalt;
            bool[] op2Inhalt = (bool[])operand2.InformationsInhalt;

            if ( op1Inhalt.Length != op2Inhalt.Length)
            {
                throw new Exception("Die Inhalte der beiden Operanden haben nicht die gleiche Länge!");
            }

            byte[] photonenArray = new byte[op1Inhalt.Length];

            // 1.Bit = Polarisationsschemata
            // 2.Bit = Schlüssel
            for (int i = 0; i < photonenArray.Length; i++)
            {
                photonenArray[i] = (byte)((op1Inhalt[i] ? (byte)1 : (byte)0) + (byte)2*(op2Inhalt[i]? (byte)1 : (byte)0));
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.photonen, photonenArray, null);
        }

        // operand1 Länge der Bitmaske
        // operand2 Anzahl der 1er in der Bitmaske
        public Information BitmaskeGenerieren(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {

            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!operand1.InformationsTyp.Equals(InformationsEnum.zahl) || !operand1.InformationsInhalt.GetType().Equals(typeof(int)) || (int)operand1.InformationsInhalt <= 0)
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
            }

            if (!operand2.InformationsTyp.Equals(InformationsEnum.zahl) || !operand2.InformationsInhalt.GetType().Equals(typeof(int)) || ((int)operand2.InformationsInhalt > (int)operand1.InformationsInhalt))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder anzahlEinser > LängeBitmaske");
            }

            int maskenLaenge = (int)operand1.InformationsInhalt;
            int anzahlEinser = (int)operand2.InformationsInhalt;

            bool[] bitArray = new bool[maskenLaenge];

            for (int i = 0; i < anzahlEinser; i++)
            {
                while (true)
                {
                    int counter = random.Next(0, maskenLaenge);
                    if (!bitArray[counter])
                    {
                        bitArray[counter] = true; 
                        break;
                    }
                }
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        // operand1 Polscharisationsschemata
        // operand2 Polscharisationsschemata
        public Information PolschataVergleichen(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ Polarisationsschemata oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ Polarisationsschemata oder ist kein bool[]");
            }

            bool[] op1 = (bool[])operand1.InformationsInhalt;
            bool[] op2 = (bool[])operand2.InformationsInhalt;

            if(op1.Length != op2.Length) 
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            bool[] erg = new bool[(op1.Length)];

            for (int i = 0; i < op1.Length; i++) 
            {
                erg[i] = op1[i] ^ op2[i];
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        // operand1 Bitfolge
        // operand2 Bitfolge
        public Information BitfolgenVergleichen(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ Bitfolge oder ist kein bool[]");
            }

            bool[] op1 = (bool[])operand1.InformationsInhalt;
            bool[] op2 = (bool[])operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            bool[] erg = new bool[op1.Length];

            for (int i = 0; i < op1.Length; i++)
            {
                erg[i] = op1[i] ^ op2[i];
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        // operand1 Bitfolge
        // operand2 null
        public Information BitfolgeNegieren(int informationsID, Information operand1, Information? operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein bool[]");
            }

            bool[] op1 = (bool[])operand1.InformationsInhalt;
            bool[] erg = new bool[op1.Length];
            for(int i = 0; i < op1.Length; i++)
            {
                erg[i] = !op1[i];
            }
            

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        // operand1 Polarisationsschemata
        // operand2 unscharfe, nicht interpretierte  Photonen
        public Information PhotonenZuBitfolge(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.unscharfePhotonen)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(byte[])))))
            {
                throw new Exception("operand2 nicht vom Typ unscharfePhotonen oder ist kein byte[]");
            }

            bool[] op1 = (bool[])operand1.InformationsInhalt;
            byte[] op2 = (byte[])operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            bool[] erg = new bool[op1.Length];

            // 1.Bit = Polarisationsschemata
            // 2.Bit = Schlüssel
            for (int i = 0; i < op1.Length; i++)
            {
                if ((op1[i] ? 1 : 0) == (op2[i] & 1))
                {
                    // Polarisationsschemata waren gleich --> es kommt das wahre Bit raus
                    erg[i] = (op2[i] & 2) > 0;
                }
                else
                {
                    // Polarisationsschemata waren ungleich --> zufälliges Bit
                    erg[i] = random.NextDouble() > 0.5;
                }
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);

        }

        // operand1 null
        // operand2 eingegebener Text
        public Information TextGenerieren(int informationsID, Information? operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!operand2.InformationsInhalt.GetType().Equals(typeof(string)))
            {
                throw new Exception("operand2 ist nicht vom Typ string");
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.asciiText, operand2.InformationsInhalt, null);
        }

        // operand1 Information (String)
        // operand2 null
        public Information TextLaengeBestimmen(int informationsID, Information operand1, Information? operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if ((!operand1.InformationsTyp.Equals(InformationsEnum.asciiText) && !operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) && !operand1.InformationsTyp.Equals(InformationsEnum.verschluesselterText) ||
                (operand1.InformationsInhalt is not (string or bool[])))
            {
                throw new Exception("operand1 nicht vom Typ asciiText oder ist kein string");
            }

            int length = -1;
            if (operand1.InformationsTyp.Equals(InformationsEnum.asciiText))
            {
                string text = (string)operand1.InformationsInhalt;
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                length = bytes.Length * 8;
            } else if (operand1.InformationsTyp.Equals(InformationsEnum.bitfolge))
            { 
                length = (operand1.InformationsInhalt as bool[]).Length;
            }else if (operand1.InformationsTyp.Equals(InformationsEnum.verschluesselterText))
            {
                string text = (string)operand1.InformationsInhalt;
                var bytes = Convert.FromBase64String(text);
                length = bytes.Length * 8;
            }

            if (length == -1)
            {
                throw new Exception("This should not be possible");
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.zahl, length, null);
        }

        // operand1 Information (String)
        // operand2 Schlüssel bestehend aus Bits
        public Information TextVerschluesseln(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.asciiText)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(string)))))
            {
                throw new Exception("operand1 nicht vom Typ Text oder ist kein String");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            string text = (string)operand1.InformationsInhalt;
            bool[] schluesselbool = (bool[])operand2.InformationsInhalt;
            BitArray schluessel = new BitArray(schluesselbool);

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            var bits = new BitArray(bytes);

            BitArray schluesselgekuerzt = new BitArray(bits.Length);

            if (bits.Length > schluessel.Length)
            {
                throw new Exception("Der Schlüssel ist zu kurz zum Verschlüsseln");
            }

            for (int i = 0; i < bits.Length; i++)
            {
                schluesselgekuerzt[i] = schluessel[i];
            }

            BitArray encryptedbits = bits.Xor(schluesselgekuerzt);
            byte[] encbyte = new byte[(((encryptedbits.Length - 1) / 8) + 1)];
            encryptedbits.CopyTo(encbyte, 0);
            string encryptedText = Convert.ToBase64String(encbyte);

            return new Information(informationsID, ergebnisName, InformationsEnum.verschluesselterText, encryptedText, null);
        }

        // operand1 verschlüsselter Text
        // operand2 Schlüssel bestehend aus Bits
        public Information TextEntschluesseln(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.verschluesselterText)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(string)))))
            {
                throw new Exception("operand1 nicht vom Typ VerschlüsselterText oder ist kein String");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            string text = (string)operand1.InformationsInhalt;
            bool[] schluesselbool = (bool[])operand2.InformationsInhalt;
            BitArray schluessel = new BitArray(schluesselbool);

            var bytes = Convert.FromBase64String(text);
            var bits = new BitArray(bytes);

            BitArray schluesselgekuerzt = new BitArray(bits.Length);

            if (bits.Length > schluessel.Length)
            {
                throw new Exception("Der Schlüssel ist zu kurz zum Entschlüsseln");
            }

            for (int i = 0; i < bits.Length; i++) 
            { 
                schluesselgekuerzt[i] = schluessel[i]; 
            }

            BitArray decryptedbits = bits.Xor(schluesselgekuerzt);
            byte[] decbyte = new byte[(((decryptedbits.Length - 1) / 8) + 1)];
            decryptedbits.CopyTo(decbyte, 0);
            Decoder d = Encoding.UTF8.GetDecoder();
            string dectext = Encoding.UTF8.GetString(decbyte);

            return new Information(informationsID, ergebnisName, InformationsEnum.asciiText, dectext, null);
        }

        // operand1 Bitfolge, aus der Bits gestrichen werden
        // operand2 Bitfolge, die angibt welche Bits gestrichen werden
        public Information BitsStreichen(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender = null)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            bool[] op1 = (bool[])operand1.InformationsInhalt;
            bool[] op2 = (bool[])operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            int zahl1er = 0;
            for(int i = 0; i<op2.Length; i++)
            {
                if (op2[i]) zahl1er++;
            }

            bool[] erg = new bool[op1.Length-zahl1er];

            int counter = 0;
            for (int i = 0; i < op1.Length; i++)
            {
                if (!op2[i])
                {
                    erg[counter++] = op1[i];
                }
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        // operand1 Angepasste Information
        // operand2 null
        public Information BitsFreiBearbeiten(int informationsID, Information operand1, Information? operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(bool[])))))
            {
                throw new Exception("operand1 nicht vom Typ bitfolge oder ist kein bool[]");
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, operand1.InformationsInhalt, null);
        }

        // operand1 null
        // operand2 Zahl per Eingabe
        public Information ZahlGenerieren(int informationsID, Information? operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null|| operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if(!operand2.InformationsInhalt.GetType().Equals(typeof(int)))
            {
                throw new Exception("operand2 ist nicht vom Typ int");
            }
            
            return new Information(informationsID, ergebnisName, InformationsEnum.zahl, operand2.InformationsInhalt, null);
        }

        // operand1 Information, die umbenannt werden soll
        // operand2 Name der Information
        public Information InformationUmbenennen(int informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            if (informationsID == null || operand1 == null || operand2 == null || ergebnisName == null)
            {
                throw new ArgumentNullException("Object reference not set to an instance of an object");
            }

            if (!operand2.InformationsInhalt.GetType().Equals(typeof(string)))
            {
                throw new Exception("operand2 ist nicht vom Typ string");
            }

            return new Information(informationsID, (string) operand2.InformationsInhalt, operand1.InformationsTyp, operand1.InformationsInhalt);
        }

        // operand1 null
        // operand2 null
        public Information? ZugBeenden(int? informationsID, Information operand1, Information operand2, String ergebnisName, RolleEnum? sender)
        {
            return null;
        }
    }
}
