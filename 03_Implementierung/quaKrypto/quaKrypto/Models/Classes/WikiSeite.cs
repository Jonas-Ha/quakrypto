using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace quaKrypto.Models.Classes
{
    public class WikiSeite : INotifyPropertyChanged
    {
        private ObservableCollection<Inline> inlineList = new();
        public ObservableCollection<Inline> InlineList { get { return inlineList; } set { inlineList = value; } }

        private static int nextAvailableIdentifier = 0;
        private readonly int identifier;

        private string wikiSeiteName = "Name der Seite";
        private bool istAktiv = false;
        private bool editierModus = false;
        private string inhalt = "Inhalt der Seite";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Identifier { get { return identifier.ToString(); } }
        public string WikiSeiteName { get { return wikiSeiteName; } set { wikiSeiteName = value; PropertyHasChanged(nameof(WikiSeiteName)); } }
        public Brush BorderBrush { get { return istAktiv ? Brushes.Black : Brushes.White; } }
        public string Inhalt { get { return inhalt; } set { inhalt = value; PropertyHasChanged(nameof(Inhalt)); } }
        public double Durchschein { get { return istAktiv ? 1.0d : editierModus ? 0.5d : 1.0d; } }

        public WikiSeite(string wikiSeiteName, string inhalt)
        {
            identifier = nextAvailableIdentifier++;
            this.wikiSeiteName = wikiSeiteName;
            this.inhalt = inhalt;
        }

        public void SetzeAktivStatus(bool aktiv)
        {
            istAktiv = aktiv;
            if (editierModus == false && istAktiv)
            {
                Trace.WriteLine("SetzeAktivStatus");
                inlineList.Clear();
                string[] inhaltZeilen = Inhalt.Split('\n');
                foreach (string zeile in inhaltZeilen)
                {
                    Match match = Regex.Match(zeile, @"[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?");
                    if (match.Success)
                    {
                        Trace.WriteLine("Match success");
                        string[] parts = zeile.Split(match.Value);
                        inlineList.Add(new Run { Text = parts[0] });
                        Hyperlink hyperlink = new(new Run(match.Value)) { NavigateUri = new Uri(match.Value.StartsWith("https://") || match.Value.StartsWith("http://")? match.Value : "https://" + match.Value) };
                        hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler((sender, e) =>
                        {
                            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true }); ;
                            e.Handled = true;
                        });
                        inlineList.Add(hyperlink);
                        inlineList.Add(new Run { Text = parts[1] });
                    }
                    else
                    {
                        inlineList.Add(new Run { Text = zeile + '\n' });
                    }

                }
            }
            PropertyHasChanged(nameof(BorderBrush));
            PropertyHasChanged(nameof(WikiSeiteName));
            PropertyHasChanged(nameof(Inhalt));
            PropertyHasChanged(nameof(InlineList));
        }
        public void SetzeEditierModus(bool neuerEditierModus)
        {
            editierModus = neuerEditierModus;
            PropertyHasChanged(nameof(Durchschein));
            if (istAktiv == true && neuerEditierModus == false)
            {
                Trace.WriteLine("EditierModus");
                inlineList.Clear();
                inlineList.Add(new Run { Text = Inhalt });
            }
            SetzeAktivStatus(istAktiv);
        }
        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
