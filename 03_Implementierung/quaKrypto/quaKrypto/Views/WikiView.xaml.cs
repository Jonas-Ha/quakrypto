// **********************************************************
// File: WikiView.xaml.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System.ComponentModel;
using quaKrypto.Models.Classes;

namespace quaKrypto.Views
{
    //Diese Klasse stellt die WikiView dar. Sie beschreibt, was das Wiki beim erstellen und schließen machen soll.
    public partial class WikiView
    {
        //Beim Erstellen des Wikis sollen die Komponenten initialisiert werden und die erste Seite des Wikis wird selektiert
        public WikiView() { InitializeComponent(); Wiki.SelektiereDieErsteSeite(); }

        //Beim Schließen des Wikis werden alle Seiten gespeichert und eine Variable wird geändert, welche aussagt, dass das Wiki nun wieder geöffnet werden kann.
        private void WikiWirdBeendet(object sender, CancelEventArgs e) { Wiki.SpeichereBenutzerWikiSeiten(); Wiki.WikiIstOffen = false; }
    }
}
