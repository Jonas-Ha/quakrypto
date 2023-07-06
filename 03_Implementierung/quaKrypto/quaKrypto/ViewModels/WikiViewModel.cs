// **********************************************************
// File: WikiViewModel.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System.Collections.ObjectModel;
using System.Windows;

namespace quaKrypto.ViewModels
{
    //Diese Klasse ist das ViewModel des Wikis.
    public class WikiViewModel : BaseViewModel
    {
        //Das sind die Properties des ViewModels, auf welche in der View gebindet wird.
        public static ObservableCollection<WikiSeite> WikiSeiten => Wiki.WikiSeiten;
        public WikiSeite SelektierteWikiSeite => Wiki.SelektierteWikiSeite;
        private bool editierModus = false;
        public bool EditierModus { get => editierModus; set { editierModus = value; EigenschaftWurdeGeändert(nameof(Cursor)); EigenschaftWurdeGeändert(nameof(EditierModus)); EigenschaftWurdeGeändert(nameof(LabelSichtbar)); EigenschaftWurdeGeändert(nameof(TextBoxSichtbar)); } }
        public Visibility LabelSichtbar => EditierModus ? Visibility.Collapsed : Visibility.Visible;
        public Visibility TextBoxSichtbar => !EditierModus ? Visibility.Collapsed : Visibility.Visible;

        public string Cursor => EditierModus ? "No" : "Hand";

        //Das sind die DelegateCommands, welche aufgerufen werden, wenn bestimmte Handlungen in der View ausgeführt werden.
        public DelegateCommand SeitenErweitern { get; set; }
        public DelegateCommand SeiteBearbeiten { get; set; }
        public DelegateCommand SeiteEntfernen { get; set; }
        public DelegateCommand SeiteSelektiert { get; set; }

        public WikiViewModel()
        {

            //Alle DelegateCommands geben die Commands weiter an das Wiki, welches sich um die Logik kümmern soll.

            //Hier fügt man eine neue Seite hinzu.
            SeitenErweitern = new((o) =>
            {
                Wiki.SeitenErweitern();
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            //Hier wurde sich entschieden, eine Seite zu entfernen
            SeiteEntfernen = new((o) =>
            {
                Wiki.SeiteEntfernen();
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            //Hier wurde eine Seite ausgewählt
            SeiteSelektiert = new((o) =>
            {
                Wiki.SeiteSelektieren(o.ToString() ?? "0");
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            //Und hier wird eine Seite bearbeitet
            SeiteBearbeiten = new((o) =>
            {
                EditierModus = !EditierModus;
                foreach (WikiSeite wikiSeite in WikiSeiten)
                {
                    //Alle Seiten bekommen den richtigen EditierModus gesetzt.
                    wikiSeite.SetzeEditierModus(EditierModus);
                }
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => true);
        }
    }
}
