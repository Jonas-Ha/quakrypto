using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace quaKrypto.Models.Classes
{
    public class WikiSeite : INotifyPropertyChanged
    {
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
            PropertyHasChanged(nameof(BorderBrush));
        }
        public void SetzeEditierModus(bool neuerEditierModus)
        {
            editierModus = neuerEditierModus;
            PropertyHasChanged(nameof(Durchschein));
        }
        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
