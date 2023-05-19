using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace quaKrypto.Models.Classes
{
    public partial class Wiki : Window, INotifyPropertyChanged
    {
        private static readonly string WIKI_ORDNERNAME = Path.Combine(Environment.CurrentDirectory, "Wiki");
        private static ObservableCollection<WikiSeite> wikiSeiten = new() { new WikiSeite("Neue Seite", "") };
        private static int indexDerSelektiertenSeite = 0;
        private static int IndexDerSelektiertenSeite
        {
            get { return indexDerSelektiertenSeite; }
            set
            {
                wikiSeiten.ElementAt(indexDerSelektiertenSeite < wikiSeiten.Count ? indexDerSelektiertenSeite : 0).SetzeAktivStatus(false);
                indexDerSelektiertenSeite = value;
                wikiSeiten.ElementAt(indexDerSelektiertenSeite).SetzeAktivStatus(true);
            }
        }
        public static ObservableCollection<WikiSeite> WikiSeiten { get { return wikiSeiten; } }
        public WikiSeite SelektierteWikiSeite { get { return wikiSeiten.ElementAt(indexDerSelektiertenSeite); } }


        public event PropertyChangedEventHandler? PropertyChanged;


        private bool editierModus = false;
        public bool EditierModus { get { return editierModus; } set { editierModus = value; PropertyHasChanged(nameof(EditierModus)); PropertyHasChanged(nameof(LabelSichtbar)); PropertyHasChanged(nameof(TextBoxSichtbar)); } }

        public Visibility LabelSichtbar { get { return EditierModus ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility TextBoxSichtbar { get { return !EditierModus ? Visibility.Collapsed : Visibility.Visible; } }

        public Wiki()
        {
            if (!Directory.Exists(WIKI_ORDNERNAME)) _ = Directory.CreateDirectory(WIKI_ORDNERNAME);
            LadeAlleWikiSeiten();
            InitializeComponent();
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite;
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }

        private void WikiSeiteSelektiert(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EditierModus) return;
            Label identifierLabel = ((StackPanel)sender).Children.OfType<Label>().First();
            IndexDerSelektiertenSeite = BekommeIndexVonIdentifier(identifierLabel.Content.ToString() ?? "0");
            PropertyHasChanged(nameof(SelektierteWikiSeite));
        }

        private void SeitenErweitern_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EditierModus) return;
            wikiSeiten.Add(new WikiSeite("Neue Seite", ""));
            IndexDerSelektiertenSeite = wikiSeiten.Count - 1;
            PropertyHasChanged(nameof(SelektierteWikiSeite));
        }
        private void SeiteBearbeiten_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditierModus = !EditierModus;
            foreach (WikiSeite wikiSeite in WikiSeiten)
            {
                wikiSeite.SetzeEditierModus(EditierModus);
            }
        }
        private void SeiteEntfernen_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EditierModus) return;
            if (IndexDerSelektiertenSeite == 0 && wikiSeiten.Count <= 1) return;
            WikiSeiten.RemoveAt(IndexDerSelektiertenSeite);
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite >= WikiSeiten.Count ? --IndexDerSelektiertenSeite : IndexDerSelektiertenSeite;
            PropertyHasChanged(nameof(SelektierteWikiSeite));
        }

        private static int BekommeIndexVonIdentifier(string identifier)
        {
            return WikiSeiten.IndexOf(WikiSeiten.Where(wikiSeite => { return wikiSeite.Identifier == identifier; }).First());
        }

        public static void SpeichereAlleWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_ORDNERNAME);
            foreach (string datei in dateien)
            {
                File.Delete(datei);
            }
            foreach (WikiSeite wikiSeite in WikiSeiten)
            {
                File.WriteAllText(Path.Combine(WIKI_ORDNERNAME, "(" + wikiSeite.Identifier + ") " + EntferneVerbotenesVonDateiNamen(wikiSeite.WikiSeiteName)), wikiSeite.Inhalt);
            }
        }

        private static void LadeAlleWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_ORDNERNAME);
            wikiSeiten = new();
            foreach (string datei in dateien)
            {
                if (File.Exists(datei))
                {
                    wikiSeiten.Add(new WikiSeite(Path.GetFileName(datei).Split(") ")[1], File.ReadAllText(datei)));
                }
            }
            if (wikiSeiten.Count == 0) wikiSeiten.Add(new WikiSeite("Neue Seite", ""));
            IndexDerSelektiertenSeite = 0;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SpeichereAlleWikiSeiten();
        }

        private static string EntferneVerbotenesVonDateiNamen(string stringWelcherBereinigtWerdenSoll)
        {
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('<', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('>', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace(':', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('"', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('/', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('\\', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('|', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('?', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('*', ' ');

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("CON", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("PRN", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("AUX", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("NUL", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM1", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM2", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM3", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM4", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM5", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM6", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM7", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM8", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM9", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT1", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT2", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT3", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT4", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT5", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT6", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT7", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT8", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT9", " ");

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Trim();
            if (stringWelcherBereinigtWerdenSoll[^1] == '.') stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll[..^1];

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace(") ", " ");
            return stringWelcherBereinigtWerdenSoll;
        }
    }
}
