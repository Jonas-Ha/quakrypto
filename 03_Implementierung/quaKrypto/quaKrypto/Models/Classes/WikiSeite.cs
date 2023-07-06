// **********************************************************
// File: WikiSeite.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace quaKrypto.Models.Classes
{
    //Diese Klasse stellt eine Datenkapselung einer WikiSeite dar.
    public class WikiSeite : INotifyPropertyChanged
    {
        //Hier sind die einzelnen Properties der Klasse aufzufinden.
        public ObservableCollection<Inline> InlineList { get; set; } = new();

        private static int nextAvailableIdentifier = 0;
        private readonly int identifier;

        private string wikiSeiteName = "Name der Seite";
        private bool istAktiv = false;
        private bool editierModus = false;
        private string inhalt = "Inhalt der Seite";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Identifier => identifier.ToString();
        public int IdentifierInteger => identifier;
        public string WikiSeiteName { get => wikiSeiteName; set { wikiSeiteName = value; PropertyHasChanged(nameof(WikiSeiteName)); } }
        public Brush BorderBrush => istAktiv ? Brushes.Black : Brushes.White;
        public string Inhalt { get => inhalt; set { inhalt = value; PropertyHasChanged(nameof(Inhalt)); } }
        public double Durchschein => istAktiv ? 1.0d : editierModus ? 0.5d : 1.0d;

        //Im Konstruktor wird der WikiSeite ein Identifier zugewiesen, genauso wie ein Name und der Inhalt
        public WikiSeite(string wikiSeiteName, string inhalt, int identifier = -1)
        {
            this.identifier = identifier != -1 ? identifier : nextAvailableIdentifier++;
            if (this.identifier >= nextAvailableIdentifier) nextAvailableIdentifier = identifier;
            this.wikiSeiteName = wikiSeiteName;
            this.inhalt = inhalt;
        }

        //In dieser Methode wird der AktivStatus der WikiSeite gesetzt.
        public void SetzeAktivStatus(bool aktiv)
        {
            istAktiv = aktiv;
            if (editierModus == false && istAktiv)
            {
                InlineList.Clear();
                string[] inhaltZeilen = Inhalt.Split('\n');
                foreach (string zeile in inhaltZeilen)
                {
                    //Hier wird auf Links überprüft und wenn welche vorhanden sind, so wird ein Hyperlink hinzugefügt, welcher Klickbar ist.
                    Match match = Regex.Match(zeile, @"[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?");
                    if (match.Success)
                    {
                        string[] parts = zeile.Split(match.Value);
                        InlineList.Add(new Run { Text = parts[0] });
                        Hyperlink hyperlink = new(new Run(match.Value)) { NavigateUri = new Uri(match.Value.StartsWith("https://") || match.Value.StartsWith("http://") ? match.Value : "https://" + match.Value) };
                        hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler((sender, e) =>
                        {
                            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true }); ;
                            e.Handled = true;
                        });
                        InlineList.Add(hyperlink);
                        InlineList.Add(new Run { Text = parts[1] });
                    }
                    else InlineList.Add(new Run { Text = zeile + '\n' });
                }
            }
            PropertyHasChanged(nameof(BorderBrush));
            PropertyHasChanged(nameof(WikiSeiteName));
            PropertyHasChanged(nameof(Inhalt));
            PropertyHasChanged(nameof(InlineList));
        }

        //In dieser Methode wird der EditierModus der WikiSeite gesetzt.
        public void SetzeEditierModus(bool neuerEditierModus)
        {
            editierModus = neuerEditierModus;
            PropertyHasChanged(nameof(Durchschein));
            //Wenn die Seite bearbeitet wird, so soll kein Hyperlink angezeigt werden.
            if (istAktiv == true && neuerEditierModus == false)
            {
                InlineList.Clear();
                InlineList.Add(new Run { Text = Inhalt });
            }
            SetzeAktivStatus(istAktiv);
        }
        private void PropertyHasChanged(string nameOfProperty) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));

        //Hier wird, nachdem alle StandardWikiSeiten geladen sind, der nächste Identifier auf 6 gesetzt, wodurch eine Unterscheidung von StandardWikiSeiten zu normalen WikiSeiten möglich ist.
        public static void StandardSeitenGeladen() => nextAvailableIdentifier = 6;
    }
}
