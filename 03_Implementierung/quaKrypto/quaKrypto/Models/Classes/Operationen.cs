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

            return new Information(informationsID, ergebnisName,operand1.InformationsTyp, operand1.InformationsInhalt, empfaenger);
        }

        public Information NachrichtEmpfangen(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        //Überlegung: Streichen der Funktion NachrichtAbhoeren, da identisch zu NachrichtEmpfangen (Leopold Bialek, Alexander Dennner)
        public  Information NachrichtAbhoeren(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            return new Information(informationsID, ergebnisName, operand1.InformationsTyp, operand1.InformationsInhalt, null);
        }

        // Rückgabe einer zufälligen Bitfolge aus einer Zufallszahl generiert
        public Information BitfolgeGenerierenZahl(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            var rand = new Random();
            int zufallszahl = (int)operand1.InformationsInhalt;

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

        public Information BitfolgeGenerierenAngabe(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
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

        public Information PolarisationsschemataGenerierenZahl(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
            }

            var rand = new Random();
            int zufallszahl = (int)operand1.InformationsInhalt;

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

        public Information PolarisationsschemataGenerierenAngabe(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
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

        public Information PhotonenGenerieren(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
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

            for (int i = 0; i < photonenArray.Length; i++)
            {
                photonenArray[i] = (byte)((op1Inhalt[i] ? (byte)1 : (byte)0) + (byte)2*(op2Inhalt[i]? (byte)1 : (byte)0));
            }

            return new Information(informationsID, ergebnisName, InformationsEnum.photonen, photonenArray, null);
        }

        public Information BitmaskeGenerieren(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {

            if (informationsID.Equals(null) || operand1.Equals(null) || operand2.Equals(null) || ergebnisName.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (!(operand1.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand1.InformationsInhalt.GetType().Equals(typeof(int)) || (((int)operand1.InformationsInhalt <= 0)))))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int oder ist <= null");
            }

            if (!(operand2.InformationsTyp.Equals(InformationsEnum.zahl)) || !((operand2.InformationsInhalt.GetType().Equals(typeof(int)))) || ((int)operand2.InformationsInhalt > (int)operand1.InformationsInhalt))
            {
                throw new Exception("operand1 nicht vom Typ Zahl oder ist kein int");
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

        public Information PolschataVergleichen(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            
        }

        public Information BitfolgenVergleichen(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PhotonenZuBitfolge(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextVerschluesseln(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextEntschluesseln(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsStreichen(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsFreiBearbeiten(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information ZugBeenden(uint informationsID, OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisName)
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }
    }
}
