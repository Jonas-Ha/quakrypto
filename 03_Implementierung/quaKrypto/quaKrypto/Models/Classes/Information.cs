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
        private string informationsName;
        private Enums.InformationsEnum informationsTyp;
        private object informationsInhalt;

        public Information(int informationsID, string informationsName, InformationsEnum informationsTyp, object informationsInhalt, RolleEnum? informationsEmpfaenger)
        {
            InformationsID = informationsID;
            InformationsName = informationsName;
            InformationsTyp = informationsTyp;
            InformationsInhalt = informationsInhalt;
            InformationsEmpfaenger = informationsEmpfaenger;
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

        public string InformationsInhaltToString()
        {
            string erg = string.Empty;
            if (InformationsInhalt == null) ;
            else if (InformationsTyp == InformationsEnum.bitfolge && InformationsInhalt.GetType() == typeof(BitArray))
            {
                BitArray bitArray = (BitArray)InformationsInhalt;
                for(int i = 0; i < bitArray.Length; i++)
                {
                    erg += bitArray[i] ? 1 : 0;
                }
            }
            return erg;
        }
    }
}
