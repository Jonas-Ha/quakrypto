// **********************************************************
// File: Aufzeichung.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    public class Aufzeichnung
    {
        private List<Handlungsschritt> handlungsschritte;

        public Aufzeichnung()
        {
            this.handlungsschritte = new List<Handlungsschritt>();
        }

        public void HaengeHandlungsschrittAn(Handlungsschritt handlungsschritt)
        {
            this.handlungsschritte.Add(handlungsschritt);
        }

        public void HaengeListeHandlungsschritteAn(List<Handlungsschritt> handlungsschritte)
        {
            this.handlungsschritte.AddRange(handlungsschritte);
        }
    }
}
