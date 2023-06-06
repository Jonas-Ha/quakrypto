﻿// **********************************************************
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
        public Information NachrichtSenden(uint informationsID, Information operand1, Object operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand2.GetType().Equals(typeof(RolleEnum))))
            {
                throw new Exception("operand2 nicht vom Typ RolleEnum");
            }

            RolleEnum empfaenger = (RolleEnum)operand2;
            
            if(operand1.InformationsTyp == InformationsEnum.photonen)
            {
                return new Information(informationsID, ergebnisName, InformationsEnum.unscharfePhotonen, operand1.InformationsInhalt, empfaenger);
            }

            return new Information(informationsID, ergebnisName,operand1.InformationsTyp, operand1.InformationsInhalt, empfaenger);
        }

        public Information NachrichtEmpfangen(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        //Überlegung: Streichen der Funktion NachrichtAbhoeren, da identisch zu NachrichtEmpfangen (Leopold Bialek, Alexander Dennner)
        public  Information NachrichtAbhoeren(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        // Rückgabe einer zufälligen Bitfolge aus einer Zufallszahl generiert
        public Information BitfolgeGenerierenZahl(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            var rand = new Random();
            int zufallszahl = (int)operand1.InformationsInhalt;

            if(zufallszahl<=0)
            {
                throw new Exception("operand1 darf nicht negativ sein");
            }

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            BitArray bitArray = new BitArray(zufallszahl, false);

            for (int i = 0; i < zufallszahl; i++)
            {
                // NextDouble gibt einen double s
                if (rand.NextDouble() < 0.5) bitArray[i] = false;
                else bitArray[i] = true;
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        public Information BitfolgeGenerierenAngabe(uint informationsID,  Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein BitArray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(int)))) || (((int)operand2.InformationsInhalt <= 0 )))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
            }

            int arrayLaenge = (int)operand2.InformationsInhalt;

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            BitArray bitArray = new BitArray(arrayLaenge, false);
            BitArray bitArrayEingegeben = (BitArray)operand1.InformationsInhalt;
            int bitArrayEingegebenLaenge = bitArrayEingegeben.Length;

            for (int i = 0; i < arrayLaenge; i++)
            {
                bitArray[i] = bitArrayEingegeben[i%bitArrayEingegebenLaenge];
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        public Information PolarisationsschemataGenerierenZahl(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            var rand = new Random();
            int zufallszahl = (int)operand1.InformationsInhalt;

            if (zufallszahl <= 0)
            {
                throw new Exception("operand1 darf nicht negativ sein");
            }

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            BitArray bitArray = new BitArray(zufallszahl, false);

            for (int i = 0; i < zufallszahl; i++)
            {
                // NextDouble gibt einen double s
                if (rand.NextDouble() < 0.5) bitArray[i] = false;
                else bitArray[i] = true;
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.polarisationsschemata, bitArray, null);
        }

        public Information PolarisationsschemataGenerierenAngabe(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein BitArray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(int)))) || (((int)operand2.InformationsInhalt <= 0)))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder der eingegebene int ist kleiner/gleich 0");
            }

            int arrayLaenge = (int)operand2.InformationsInhalt;

            // Anlegen eines Bit-Arrays der Länge von Zufallszahl
            BitArray bitArray = new BitArray(arrayLaenge, false);
            BitArray bitArrayEingegeben = (BitArray)operand1.InformationsInhalt;
            int bitArrayEingegebenLaenge = bitArrayEingegeben.Length;

            for (int i = 0; i < arrayLaenge; i++)
            {
                bitArray[i] = bitArrayEingegeben[i % bitArrayEingegebenLaenge];
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.polarisationsschemata, bitArray, null);
        }

        //Operand 1 = polarisationsschemata, Operand2 = Schlüssel
        public Information PhotonenGenerieren(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ polarisationsschemata oder ist kein BitArray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            BitArray op1Inhalt = (BitArray)operand1.InformationsInhalt;
            BitArray op2Inhalt = (BitArray)operand2.InformationsInhalt;

            if ( op1Inhalt.Length != op2Inhalt.Length)
            {
                throw new Exception("Die Inhalte der beiden Operanden haben nicht die gleiche Länge!");
            }

            byte[] photonenArray = new byte[op1Inhalt.Length];

            //21 (Photonen)
            //XX 1.Bit = Polarisationsschmata, 2.Bit = Schlüssel
            for (int i = 0; i < photonenArray.Length; i++)
            {
                photonenArray[i] = (byte)((op1Inhalt[i] ? (byte)1 : (byte)0) + (byte)2*(op2Inhalt[i]? (byte)1 : (byte)0));
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.photonen, photonenArray, null);
        }

        public Information BitmaskeGenerieren(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {

            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!operand1.InformationsTyp.Equals(InformationsEnum.zahl) || !operand1.InformationsInhalt.GetType().Equals(typeof(int)) || (int)operand1.InformationsInhalt <= 0)
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
            }

            if (!operand2.InformationsTyp.Equals(InformationsEnum.zahl) || !operand2.InformationsInhalt.GetType().Equals(typeof(int)) || ((int)operand2.InformationsInhalt > (int)operand1.InformationsInhalt))
            {
                throw new Exception("operand2 nicht vom Typ Zahl oder ist kein int oder anzahlEinser > LängeBitmaske");
            }

            var rand = new Random();
            int maskenLaenge = (int)operand1.InformationsInhalt;
            int anzahlEinser = (int)operand2.InformationsInhalt;

            BitArray bitArray = new BitArray(maskenLaenge, false);

            for (int i = 0; i < anzahlEinser; i++)
            {
                while (true)
                {
                    int counter = rand.Next(0, maskenLaenge);
                    if (!bitArray[counter])
                    {
                        bitArray[counter] = true; 
                        break;
                    }
                }
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, bitArray, null);
        }

        public Information PolschataVergleichen(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ Polarisationsschemata oder ist kein Bitarray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ Polarisationsschemata oder ist kein Bitarray");
            }
            
            BitArray op1 = (BitArray)operand1.InformationsInhalt;
            BitArray op2 = (BitArray)operand2.InformationsInhalt;

            if(op1.Length != op2.Length) 
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            BitArray erg = new BitArray(op1.Length);

            for (int i = 0; i < op1.Length; i++) 
            {
                erg[i] = op1[i] ^ op2[i];
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        public Information BitfolgenVergleichen(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein Bitarray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ Bitfolge oder ist kein Bitarray");
            }

            BitArray op1 = (BitArray)operand1.InformationsInhalt;
            BitArray op2 = (BitArray)operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            BitArray erg = new BitArray(op1.Length);

            for (int i = 0; i < op1.Length; i++)
            {
                erg[i] = op1[i] ^ op2[i];
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        public Information BitfolgeNegieren(uint informationsID, Information operand1, Information? operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ Bitfolge oder ist kein Bitarray");
            }

            BitArray op1 = (BitArray)operand1.InformationsInhalt;

            BitArray erg = op1.Not();

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);
        }

        //Operand1 = Polarisationsschemata, Operand2 = unscharfePhotonen
        public Information PhotonenZuBitfolge(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.polarisationsschemata)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ polarisationsschmata oder ist kein Bitarray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.unscharfePhotonen)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(byte[])))))
            {
                throw new Exception("operand2 nicht vom Typ unscharfePhotonen oder ist kein byte[]");
            }

            BitArray op1 = (BitArray)operand1.InformationsInhalt;
            byte[] op2 = (byte[])operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            var rand = new Random();

            BitArray erg = new BitArray(op1.Length);

            //21 (Photonen)
            //XX 1.Bit = Polarisationsschmata, 2.Bit = Schlüssel
            for (int i = 0; i < op1.Length; i++)
            {
                if ((op1[i] ? 1 : 0) == (op2[i] & 1))
                {
                    //Polarisationsschemata waren gleich => es kommt das wahre Bit raus
                    erg[i] = (op2[i] & 1) > 0;
                }
                else
                {
                    //Polarisationsschemata waren ungleich => zufälliges Bit
                    erg[i] = rand.NextDouble() > 0.5;
                }
            }
            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, erg, null);

        }

        public Information TextGenerieren(uint informationsID, Information? operand1, object operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!operand2.GetType().Equals(typeof(string)))
            {
                throw new Exception("operand2 ist nicht vom Typ string");
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.asciiText, operand2, null);
        }

        public Information TextLaengeBestimmen(uint informationsID, Information operand1, Information? operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.asciiText)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(string)))))
            {
                throw new Exception("operand1 nicht vom Typ asciiText oder ist kein string");
            }

            string text = (string)operand1.InformationsInhalt;
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            var bits = new BitArray(bytes);
            
            return new Information(informationsID, ergebnisName, InformationsEnum.zahl, bits.Length, null);
        }

        //Operand1 = Text, Operand2 = Schlüssel
        public Information TextVerschluesseln(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.asciiText)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(string)))))
            {
                throw new Exception("operand1 nicht vom Typ Text oder ist kein String");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            string text = (string)operand1.InformationsInhalt;
            BitArray schluessel = (BitArray)operand2.InformationsInhalt;
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

        public Information TextEntschluesseln(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.verschluesselterText)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(string)))))
            {
                throw new Exception("operand1 nicht vom Typ VerschlüsselterText oder ist kein String");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            string text = (string)operand1.InformationsInhalt;
            BitArray schluessel = (BitArray)operand2.InformationsInhalt;
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

        public Information BitsStreichen(uint informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand2 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            BitArray op1 = (BitArray)operand1.InformationsInhalt;
            BitArray op2 = (BitArray)operand2.InformationsInhalt;

            if (op1.Length != op2.Length)
            {
                throw new Exception("operand1 und operand2 sind nicht gleich lang");
            }

            int zahl1er = 0;
            for(int i = 0; i<op2.Length; i++)
            {
                if (op2[i]) zahl1er++;
            }

            BitArray erg = new BitArray(op1.Length-zahl1er);

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

        public Information BitsFreiBearbeiten(uint informationsID, Information operand1, Information? operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.bitfolge)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(BitArray)))))
            {
                throw new Exception("operand1 nicht vom Typ bitfolge oder ist kein BitArray");
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.bitfolge, operand1.InformationsInhalt, null);
        }

        public Information ZahlGenerieren(uint informationsID, Information? operand1, object operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if(!operand2.GetType().Equals(typeof(int)))
            {
                throw new Exception("operand2 ist nicht vom Typ int");
            }
            
            return new Information(informationsID, ergebnisName, InformationsEnum.zahl, operand2, null);
        }

        public Information? ZugBeenden(uint? informationsID, Information operand1, Information operand2, String ergebnisName)
        {
            return null;
        }
    }
}
