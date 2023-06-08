// **********************************************************
// File: Aufzeichung.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    public class Aufzeichnung
    {
        private ObservableCollection<Handlungsschritt> handlungsschritte;

        public Aufzeichnung()
        {
            this.handlungsschritte = new ObservableCollection<Handlungsschritt>();
        }

        public ObservableCollection<Handlungsschritt>? Handlungsschritte
        {
            get { return handlungsschritte; }
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
