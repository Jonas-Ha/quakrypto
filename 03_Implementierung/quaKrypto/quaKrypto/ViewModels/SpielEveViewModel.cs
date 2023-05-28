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
    public class SpielEveViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;

        public ObservableCollection<string> BituebertragungEingangAlice { get; set; }
        public ObservableCollection<string> BituebertragungEingangBob { get; set; }
        public ObservableCollection<string> PhotonenuebertragungEingangAlice { get; set; }
        public ObservableCollection<string> PhotonenuebertragungEingangBob { get; set; }
        public ObservableCollection<string> BituebertragungAusgangAlice { get; set; }
        public ObservableCollection<string> BituebertragungAusgangBob { get; set; }
        public ObservableCollection<string> PhotonenuebertragungAusgangAlice { get; set; }
        public ObservableCollection<string> PhotonenuebertragungAusgangBob { get; set; }
        public string CraftingFeldPhotonen { get; set; }
        public string CraftingFeldPolarisation { get; set; }
        public string CraftingFeldErgebnis { get; set; }
        public string Muelleimer { get; set; }
        public string Informationsablage { get; set; }


        public DelegateCommand HauptMenu { get; set; }
        public SpielEveViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);

            this.BituebertragungEingangAlice = new ObservableCollection<string>();
            this.BituebertragungEingangBob = new ObservableCollection<string>();
            this.PhotonenuebertragungEingangAlice = new ObservableCollection<string>();
            this.PhotonenuebertragungEingangBob = new ObservableCollection<string>();


            this.BituebertragungAusgangAlice = new ObservableCollection<string>();
            this.BituebertragungAusgangBob = new ObservableCollection<string>();

            this.PhotonenuebertragungAusgangAlice = new ObservableCollection<string>();
            this.PhotonenuebertragungAusgangBob = new ObservableCollection<string>();


            this.BituebertragungEingangAlice.Add("Simon");
            this.BituebertragungEingangBob.Add("Moeez");

            this.PhotonenuebertragungEingangAlice.Add("Alex");
            this.PhotonenuebertragungEingangBob.Add("Mentel");

            this.BituebertragungAusgangAlice.Add("Jonas");
            this.BituebertragungAusgangBob.Add("Leo");

            this.PhotonenuebertragungAusgangAlice.Add("Domi");
            this.PhotonenuebertragungAusgangBob.Add("Kris");
        }
    }
}
