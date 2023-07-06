// **********************************************************
// File: Information.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using quaKrypto.Models.Enums;
using System;
using System.Text;
using System.Xml.Serialization;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Information
    {
        /*
         * Eine Rolle kann in der Oberfläche mehrere Informationen anlegen.
         * Die möglichen Typen sind dem Enum <InformationsEnum> zu entnehmen (bspw. zahl, bitfolge, ...)
         * Jede erzeugte Information hat eine eindeutige ID, einen Sender und einen Empfänger. 
         */
        private int informationsID;
        private RolleEnum? informationsSender;
        private RolleEnum? informationsEmpfaenger;
        
        private string informationsName;
        // "Datentyp" der Information
        private InformationsEnum informationsTyp;
        // eigentlicher Inhalt der Information abhängig vom <informationsTyp> interpretierbar
        private object informationsInhalt;

        public Information() { }

        public Information(int informationsID, string informationsName, InformationsEnum informationsTyp, object informationsInhalt, RolleEnum? informationsEmpfaenger = null, RolleEnum? informationsSender = null)
        {
            InformationsID = informationsID;

            if (informationsEmpfaenger != null)
                InformationsEmpfaenger = informationsEmpfaenger;
            if (informationsSender != null)
                InformationsSender = informationsSender;

            InformationsName = informationsName;
            InformationsTyp = informationsTyp;
            InformationsInhalt = informationsInhalt;
        }

        public int InformationsID
        {
            get { return informationsID; }
            init { informationsID = value; }
        }

        public string InformationsName
        {
            get { return informationsName; }
            init { informationsName = value; }
        }

        public InformationsEnum InformationsTyp
        {
            get { return informationsTyp; }
            init { informationsTyp = value; }
        }

        [XmlIgnore]
        public object InformationsInhalt
        {
            get { return informationsInhalt; }
            init { informationsInhalt = value; }
        }

        // gibt an, ob die Information abhängig vom Empfänger serialisiert werden soll
        public bool ShouldSerializeInformationsEmpfaenger()
        {
            return informationsEmpfaenger != null;
        }

        public RolleEnum? InformationsEmpfaenger
        {
            get { return informationsEmpfaenger; }
            init { informationsEmpfaenger = value; }
        }

        // gibt an, ob die Information abhängig vom Sender serialisiert werden soll
        public bool ShouldSerializeInformationsSender()
        {
            return InformationsSender != null;
        }

        public RolleEnum? InformationsSender
        {
            get { return informationsSender; }
            init { informationsSender = value; }
        }

        // Darstellung des Informationsname der Information für die Oberfläche
        [XmlIgnore]
        public string InformationsNameToString
        {
            get
            {
                string erg = string.Empty;

                if ((InformationsName == "ManuelleEingabeZahl") || (InformationsName == "ManuelleEingabeBitfolge")) return erg;
                else erg = informationsName;

                return erg;
            }
        }

        // Darstellung der Information abhängig vom <informationsTyp> der Information für die Oberfläche
        [XmlIgnore]
        public string InformationsInhaltToString
        {
            get
            {
                // Rückgabe des Ergebnisses als String, der auf der Oberfläche angezeigt werden kann
                string erg = string.Empty;

                // kein Informationsinhalt verfügbar
                // --> Rückgabe eines leeren Strings
                if (InformationsInhalt == null) return erg;

                // Information ist eines Zahl
                else if (InformationsTyp == InformationsEnum.zahl && InformationsInhalt.GetType() == typeof(int))
                {
                    erg = ((int)InformationsInhalt).ToString();
                }

                // Information ist eine Bitfolge (Array aus '0' & '1')
                else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }

                // Information ist eine Folge aus Photonen
                else if (InformationsTyp == InformationsEnum.photonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] photonenArray = (byte[])InformationsInhalt;
                    erg = Photonen_ToString(photonenArray);
                }

                // Information sind Polarisationsschemata
                // werden als '0' & '1' abgespeichert und als '✛' & '✕' angezeigt
                else if (InformationsTyp == InformationsEnum.polarisationsschemata && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? '✛' : '✕';
                    }
                }

                // unscharfe Photonen sind nicht interpretierte Photonen und müssen mit Polarisationsschemata zuerst interpretiert werden
                // werden als '*'
                else if (InformationsTyp == InformationsEnum.unscharfePhotonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] photonenArray = (byte[])InformationsInhalt;
                    erg = new string('*', photonenArray.Length);
                }

                // String
                else if (InformationsTyp == InformationsEnum.asciiText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }

                // verschlüsselter Text; vergleichbar mit String
                else if (InformationsTyp == InformationsEnum.verschluesselterText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }

                return erg;
            }
        }

        // Serialisierung des Informationsinhalt zum Übertragen der Information über das Netzwerk
        public string InformationsinhaltSerialized
        {
            get
            {
                string erg = string.Empty;

                // Information ist eines Zahl
                if (InformationsTyp == InformationsEnum.zahl && InformationsInhalt.GetType() == typeof(int))
                {
                    erg = ((int)InformationsInhalt).ToString();
                }

                // Information ist eine Bitfolge (Array aus '0' & '1')
                else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }

                // Information ist eine Folge aus Photonen
                else if (InformationsTyp == InformationsEnum.photonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] byteArr = (byte[])InformationsInhalt;
                    for (int i = 0; i < ((byte[])InformationsInhalt).Length; i++)
                    {
                        erg += byteArr[i].ToString();
                    }
                }

                // Information sind Polarisationsschemata
                // werden als '0' & '1' abgespeichert und als '0' & '1' übertragen
                else if (InformationsTyp == InformationsEnum.polarisationsschemata && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }

                // unscharfe Photonen sind nicht interpretierte Photonen und müssen mit Polarisationsschemata zuerst interpretiert werden
                // werden als Bytes übertragen (vier Varianten von Photonen)
                else if (InformationsTyp == InformationsEnum.unscharfePhotonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] byteArr = (byte[])InformationsInhalt;
                    for (int i = 0; i < ((byte[])InformationsInhalt).Length; i++)
                    {
                        erg += byteArr[i].ToString();
                    }
                }

                // String
                // wird als Base64 String übertragen
                else if (InformationsTyp == InformationsEnum.asciiText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = Convert.ToBase64String(Encoding.UTF8.GetBytes((string)InformationsInhalt));
                }

                // verschlüsselter Text; vergleichbar mit String
                // wird als Base64 String übertragen
                else if (InformationsTyp == InformationsEnum.verschluesselterText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = Convert.ToBase64String(Encoding.UTF8.GetBytes((string)InformationsInhalt));
                }

                if (InformationsInhalt == null) return "";

                // Übertragen des Informationsinhalts <erg> mit dem dazugehörigen Datentypen
                return $"{InformationsInhalt.GetType()}>{erg}";
            }
            set
            {
                // Separierung des Informationsinhalts und des dazugehörigen Datentypen
                string[] teile = value.Split(">");

                // Ermittlung des Datentypen
                Type? type = Type.GetType(teile[0]);

                // Ermittlung des Informationsinhalts
                if (type != null)
                {
                    // Datentyp ist ein Array aus '0' & '1'
                    if (type.Equals(new bool[0].GetType()))
                    {
                        bool[] returnArray = new bool[teile[1].Length];
                        for(int i = 0; i < returnArray.Length; i++)
                        {
                            returnArray[i] = teile[1][i] == '1';
                        }
                        informationsInhalt = returnArray;
                    }

                    // Datentyp ist ein Array aus Bytes
                    else if (type.Equals(new byte[0].GetType()))
                    {
                        byte[] returnArray = new byte[teile[1].Length];
                        for (int i = 0; i < returnArray.Length; i++)
                        {
                            returnArray[i] = byte.TryParse(new string (new char[1] { teile[1][i] }), out byte res) ? res : (byte)0;
                        }
                        informationsInhalt = returnArray;
                    }

                    // Datentyp ist ein String
                    else if (type.Equals(new string("").GetType()))
                    {
                        informationsInhalt = Encoding.UTF8.GetString(Convert.FromBase64String(teile[1]));
                    }

                    // Datentyp ist ein Integer
                    else if (type.Equals(new int().GetType()))
                    {
                        informationsInhalt = int.TryParse(teile[1], out int integer) ? integer : -1;
                    }
                }

            }
        }

        // Intepretation der Photonen als String
        private string Photonen_ToString(byte[] photonenArray)
        {
            string erg = string.Empty;

            for (int i = 0; i < photonenArray.Length; i++)
            {
                // 1.Bit = Polarisationsschemata
                // 2.Bit = Schlüssel
                if ((photonenArray[i] & 0x01) == 1)
                {
                    if ((photonenArray[i] & 0x02) == 2)
                    {
                        erg += '─'; //0x03
                    }
                    else
                    {
                        erg += '│'; //0x01
                    }
                }
                else
                {
                    if ((photonenArray[i] & 0x02) == 2)
                    {
                        erg += '╱'; //0x02
                    }
                    else
                    {
                        erg += '╲'; //0x00
                    }
                }
            }

            return erg;
        }
    }
}
