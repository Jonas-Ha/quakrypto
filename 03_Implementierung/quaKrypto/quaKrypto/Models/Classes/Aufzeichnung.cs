// **********************************************************
// File: Aufzeichung.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System.Collections.ObjectModel;

namespace quaKrypto.Models.Classes
{
    public class Aufzeichnung
    {
        // dynamische Datensammlung aus den ausgeführten Handlungsschritten aller ausgeführten Handlungsschritte
        // --> diese Datensammlung wird in der Aufzeichnung bei Beendigung eines Übungsszenarios angezeigt
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
    }
}
