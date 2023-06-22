// **********************************************************
// File: Information.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using quaKrypto.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Information
    {
        private int informationsID;
        private RolleEnum? informationsEmpfaenger;
        private RolleEnum? informationsSender;
        private string informationsName;
        private InformationsEnum informationsTyp;
        private object informationsInhalt;

        public Information() { }

        public Information(int informationsID, string informationsName, InformationsEnum informationsTyp, object informationsInhalt, RolleEnum? informationsEmpfaenger = null, RolleEnum? informationsSender = null)
        {
            InformationsID = informationsID;
            InformationsName = informationsName;
            InformationsTyp = informationsTyp;
            InformationsInhalt = informationsInhalt;
            if (informationsEmpfaenger != null)
                InformationsEmpfaenger = informationsEmpfaenger;
            if (informationsSender != null)
                InformationsSender = informationsSender;
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

        public bool ShouldSerializeInformationsEmpfaenger()
        {
            return informationsEmpfaenger != null;
        }

        public RolleEnum? InformationsEmpfaenger
        {
            get { return informationsEmpfaenger; }
            init { informationsEmpfaenger = value; }
        }
        public bool ShouldSerializeInformationsSender()
        {
            return InformationsSender != null;
        }

        public RolleEnum? InformationsSender
        {
            get { return informationsSender; }
            init { informationsSender = value; }
        }

        [XmlIgnore]
        public string InformationsInhaltToString
        {
            get
            {
                string erg = string.Empty;
                if (InformationsInhalt == null) return erg;
                else if (InformationsTyp == InformationsEnum.zahl && InformationsInhalt.GetType() == typeof(int))
                {
                    erg = ((int)InformationsInhalt).ToString();
                }
                else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }
                else if (InformationsTyp == InformationsEnum.photonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] photonenArray = (byte[])InformationsInhalt;
                    erg = Photonen_ToString(photonenArray);
                }
                else if (InformationsTyp == InformationsEnum.polarisationsschemata && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? '✛' : '✕';
                    }
                }
                else if (InformationsTyp == InformationsEnum.unscharfePhotonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] photonenArray = (byte[])InformationsInhalt;
                    erg = new string('*', photonenArray.Length);
                }
                else if (InformationsTyp == InformationsEnum.asciiText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }
                else if (InformationsTyp == InformationsEnum.verschluesselterText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }
                return erg;
            }
        }

        public string InformationsinhaltSerialized
        {
            get
            {
                string erg = string.Empty;
                if (InformationsTyp == InformationsEnum.zahl && InformationsInhalt.GetType() == typeof(int))
                {
                    erg = ((int)InformationsInhalt).ToString();
                }
                else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }
                else if (InformationsTyp == InformationsEnum.photonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] byteArr = (byte[])InformationsInhalt;
                    for (int i = 0; i < ((byte[])InformationsInhalt).Length; i++)
                    {
                        erg += byteArr[i].ToString();
                    }
                }
                else if (InformationsTyp == InformationsEnum.polarisationsschemata && InformationsInhalt.GetType() == typeof(bool[]))
                {
                    bool[] bitArray = (bool[])InformationsInhalt;
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        erg += bitArray[i] ? 1 : 0;
                    }
                }
                else if (InformationsTyp == InformationsEnum.unscharfePhotonen && InformationsInhalt.GetType() == typeof(byte[]))
                {
                    byte[] byteArr = (byte[])InformationsInhalt;
                    for (int i = 0; i < ((byte[])InformationsInhalt).Length; i++)
                    {
                        erg += byteArr[i].ToString();
                    }
                }
                else if (InformationsTyp == InformationsEnum.asciiText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }
                else if (InformationsTyp == InformationsEnum.verschluesselterText && InformationsInhalt.GetType() == typeof(string))
                {
                    erg = (string)InformationsInhalt;
                }
                return $"{InformationsInhalt.GetType()}{(char)243}{erg}";
            }
            set
            {
                string[] teile = value.Split((char)243);

                Type? type = Type.GetType(teile[0]);
                if (type != null)
                {
                    if (type.Equals(new bool[0].GetType()))
                    {
                        bool[] returnArray = new bool[teile[1].Length];
                        for(int i = 0; i < returnArray.Length; i++)
                        {
                            returnArray[i] = teile[1][i] == '1';
                        }
                        informationsInhalt = returnArray;
                    }
                    else if (type.Equals(new byte[0].GetType()))
                    {
                        byte[] returnArray = new byte[teile[1].Length];
                        for (int i = 0; i < returnArray.Length; i++)
                        {
                            returnArray[i] = byte.TryParse(new string (new char[1] { teile[1][i] }), out byte res) ? res : (byte)0;
                        }
                        informationsInhalt = returnArray;
                    }
                    else if (type.Equals(new string("").GetType()))
                    {
                        informationsInhalt = teile[1];
                    }
                    else if (type.Equals(new int().GetType()))
                    {
                        informationsInhalt = int.TryParse(teile[1], out int integer) ? integer : -1;
                    }
                }

            }
        }

        private string Photonen_ToString(byte[] photonenArray)
        {
            string erg = string.Empty;
            for (int i = 0; i < photonenArray.Length; i++)
            {
                //21 (Photonen)
                //XX 1.Bit = Polarisationsschmata, 2.Bit = Schlüssel
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
