// **********************************************************
// File: IVariante.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Automation;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Interfaces
{
    //Dieses Interface wird genutzt um eine Variante eines Protokolls zu implementieren
    //Sie stellt Funktionen und Eigenschaften bereit die den aktuellen Zustand einer Variante widerspiegeln und verändern.
    public interface IVariante : INotifyPropertyChanged
    {

        public uint AktuellePhase
        { get; }

        public RolleEnum AktuelleRolle
        { 
            get;
            set; 
        }

        //Der Name der Variante die implementiert wird
        public static string VariantenName
        {
            get { throw new ElementNotAvailableException("Element of Interface should not be accessed!"); }
        }

        //Der Name des zugehörigen Protokolls
        public string ProtokollName 
        { get; }

        //Eine Liste an Rollen, die in dieser Variante mitmachen können
        public IList<RolleEnum> MöglicheRollen
        { get; }

        //Diese Funktion berechnet die nächste Rolle die an der Reihe ist
        public RolleEnum NächsteRolle();

        //Diese Funktion gibt eine Hilfestellung zurück basierend auf einem gegebenem Schwierigkeitsgrad
        //Es wird eine Liste mit möglichen Operationen zurückgegeben.
        //Je nach Schwierigkeit sind diese Operation mehr oder weniger spezifisch/ausführlich
        public List<OperationsEnum> GebeHilfestellung(SchwierigkeitsgradEnum schwierigkeitsgrad);

        //Diese Funktion ist ein CollectionChanged CallbackFunktion
        //Sie wird aufgerufen, wenn ein neuer Handlungsschritt ausgeführt wurde
        //Sie kann basierend auf den bisherigen Handlungsschritten die aktuelle Phase im Protokoll setzen
        public void BerechneAktuellePhase(object? sender, NotifyCollectionChangedEventArgs e);
    }
}
