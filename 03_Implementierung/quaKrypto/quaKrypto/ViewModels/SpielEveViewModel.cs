using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class SpielEveViewModel : SpielViewModelBase
    {
        private SpielViewModel spielViewModel;
        private bool once = false;
        public SpielViewModel SpielViewModel
        {
            set
            {
                if (once)
                {
                    spielViewModel = value;
                    once = true;
                }
            }
        }
        public ObservableCollection<Information> BituebertragungEingangAlice { get; set; }
        public ObservableCollection<Information> BituebertragungEingangBob { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungEingangAlice { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungEingangBob { get; set; }
        public ObservableCollection<Information> BituebertragungAusgangAlice { get; set; }
        public ObservableCollection<Information> BituebertragungAusgangBob { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungAusgangAlice { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungAusgangBob { get; set; }

        public SpielEveViewModel(Navigator navigator, IUebungsszenario uebungsszenario, List<Rolle> eigeneRollen) : base(navigator, uebungsszenario, eigeneRollen)
        {
            this.BituebertragungEingangAlice = new ObservableCollection<Information>();
            this.BituebertragungEingangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangBob = new ObservableCollection<Information>();


            this.BituebertragungAusgangAlice = new ObservableCollection<Information>();
            this.BituebertragungAusgangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangBob = new ObservableCollection<Information>();
        }
    }
}
