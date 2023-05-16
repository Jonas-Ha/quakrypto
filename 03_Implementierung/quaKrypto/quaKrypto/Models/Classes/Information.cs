// **********************************************************
// File: InformationsEnum.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    internal class Information
    {
        private int informationsID;
        private string informationsName;
        private Enums.InformationsEnum informationsTyp;
        private string informationsInhalt;

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

        public string InformationsInhalt
        {
            get { return informationsInhalt; }
            init { informationsInhalt = value; }
        }
    }
}
