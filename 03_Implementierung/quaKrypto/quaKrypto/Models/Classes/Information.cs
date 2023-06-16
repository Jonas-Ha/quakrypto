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

namespace quaKrypto.Models.Classes
{
    public class Information
    {
        private int informationsID;
        private RolleEnum? informationsEmpfaenger;
        private RolleEnum? informationsSender;
        private string informationsName;
        private Enums.InformationsEnum informationsTyp;
        private object informationsInhalt;

        public Information(int informationsID, string informationsName, InformationsEnum informationsTyp, object informationsInhalt, RolleEnum? informationsEmpfaenger = null, RolleEnum? informationsSender = null)
        {
            InformationsID = informationsID;
            InformationsName = informationsName;
            InformationsTyp = informationsTyp;
            InformationsInhalt = informationsInhalt;
            if (informationsEmpfaenger != null )
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

        public Enums.InformationsEnum InformationsTyp 
        { 
            get { return informationsTyp; } 
            init { informationsTyp = value; }
        }

        public object InformationsInhalt
        {
            get { return informationsInhalt; }
            init { informationsInhalt = value; }
        }

        public RolleEnum? InformationsEmpfaenger
        {
            get { return informationsEmpfaenger; }
            init { informationsEmpfaenger = value; }
        }

        public RolleEnum? InformationsSender
        {
            get { return informationsSender; }
            init { informationsSender = value; }
        }

        public string InformationsInhaltToString
        {
            get
            {
                string erg = string.Empty;
                if (InformationsInhalt == null) ;
                else if (InformationsTyp == InformationsEnum.zahl && InformationsInhalt.GetType() == typeof(int))
                {
                    erg = ((int)InformationsInhalt).ToString();
                }
                else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(BitArray))
                {
                    BitArray bitArray = (BitArray)InformationsInhalt;
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
                else if (InformationsTyp == InformationsEnum.polarisationsschemata && InformationsInhalt.GetType() == typeof(BitArray))
                {
                    BitArray bitArray = (BitArray)InformationsInhalt;
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
