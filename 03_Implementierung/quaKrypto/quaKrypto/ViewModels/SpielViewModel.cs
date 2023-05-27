using quaKrypto.Commands;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class SpielViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;

        public ObservableCollection<string> BituebertragungEingang { get; set; }
        public ObservableCollection<string> PhotonenuebertragungEingang { get; set; }
        public ObservableCollection<string> BituebertragungAusgang { get; set; }
        public ObservableCollection<string> PhotonenuebertragungAusgang { get; set; }
        public string CraftingFeldPhotonen { get; set; }
        public string CraftingFeldPolarisation { get; set; }
        public string CraftingFeldErgebnis { get; set; }
        public string Muelleimer { get; set; }
        public string Informationsablage { get; set; }

        public DelegateCommand HauptMenu { get; set; }
        public SpielViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);

            this.BituebertragungEingang = new ObservableCollection<string>();
            this.PhotonenuebertragungEingang = new ObservableCollection<string>();

            this.BituebertragungAusgang = new ObservableCollection<string>();
            this.PhotonenuebertragungAusgang = new ObservableCollection<string>();

            this.BituebertragungEingang.Add("Simon");
            this.BituebertragungEingang.Add("Moeez");

            this.PhotonenuebertragungEingang.Add("Alex");
            this.PhotonenuebertragungEingang.Add("Mentel");

            this.BituebertragungAusgang.Add("Jonas");
            this.BituebertragungAusgang.Add("Leo");

            this.PhotonenuebertragungAusgang.Add("Domi");
            this.PhotonenuebertragungAusgang.Add("Kris");

        }
    }
}
