
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using quaKrypto.Commands;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.ViewModels
{
    public class LobbyScreenViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;
        private string _lobbyname;
        private string _variante;
        private string _schwierigkeit;
        private string _phase;
        private string _protokoll;
        private string _passwortalice = "";
        private string _aliasalice = "";
        private string _passwortbob = "";
        private string _aliasbob = "";
        private string _passworteve = "";
        private string _aliaseve = "";

        private Visibility _aliceboxesvisible = Visibility.Visible;
        private Visibility _aliceselected = Visibility.Collapsed;
        private Visibility _bobboxesvisible = Visibility.Visible;
        private Visibility _bobselected = Visibility.Collapsed;
        private Visibility _eveboxesvisible = Visibility.Visible;
        private Visibility _eveselected = Visibility.Collapsed;
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand Alicebestaetigen { get; set; }
        public DelegateCommand Bobbestaetigen { get; set; }
        public DelegateCommand Evebestaetigen { get; set; }
        public DelegateCommand ClearAlice { get; set; }
        public DelegateCommand ClearBob { get; set; }
        public DelegateCommand ClearEve { get; set; }
        public LobbyScreenViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;

            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
            Alicebestaetigen = new((o) =>
            {
                AliceCommand((o));
            }, (o) => _passwortalice != "" && _aliasalice != "");
            Bobbestaetigen = new((o) =>
            {
                BobCommand((o));
            }, (o) => _passwortbob != "" && _aliasbob != "");
            Evebestaetigen = new((o) =>
            {
                EveCommand((o));
            }, (o) => _passworteve != "" && _aliaseve != "");
            ClearAlice = new DelegateCommand(AliceFreigeben);
            ClearBob = new DelegateCommand(BobFreigeben);
            ClearEve = new DelegateCommand(EveFreigeben);
            LobbyName = uebungsszenario.Name;
            if (uebungsszenario.Variante.ToString().Contains("VarianteNormalerAblauf"))
            {
                Variante = VarianteNormalerAblauf.VariantenName;
            }
            else if (uebungsszenario.Variante.ToString().Contains("VarianteAbhoeren"))
            {
                Variante = VarianteAbhoeren.VariantenName;
            }
            else
            {
                Variante = VarianteManInTheMiddle.VariantenName;
            }
            //Variante = uebungsszenario.Variante.ToString();
            Schwierigkeit = uebungsszenario.Schwierigkeitsgrad.ToString();
            Phase = uebungsszenario.StartPhase.ToString() + " - " + uebungsszenario.EndPhase.ToString();
            Protokoll = "BB84";

        }
        public string LobbyName
        {
            get { return _lobbyname; }
            set
            {
                _lobbyname = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public string Variante
        {
            get { return _variante; }
            set
            {
                _variante = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public string Schwierigkeit
        {
            get { return _schwierigkeit; }
            set
            {
                _schwierigkeit = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public string Phase
        {
            get { return _phase; }
            set
            {
                _phase = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public string Protokoll
        {
            get { return _protokoll; }
            set
            {
                _protokoll = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public string AliasAliceText
        {
            get { return _aliasalice; }
            set
            {
                _aliasalice = value;
                this.EigenschaftWurdeGeändert();
                Alicebestaetigen.RaiseCanExecuteChanged();
            }
        }
        public string PasswortAliceText
        {
            get { return _passwortalice; }
            set
            {
                _passwortalice = value;
                this.EigenschaftWurdeGeändert();
                Alicebestaetigen.RaiseCanExecuteChanged();
            }
        }
        public string AliasBobText
        {
            get { return _aliasbob; }
            set
            {
                _aliasbob = value;
                this.EigenschaftWurdeGeändert();
                Bobbestaetigen.RaiseCanExecuteChanged();
            }
        }
        public string PasswortBobText
        {
            get { return _passwortbob; }
            set
            {
                _passwortbob = value;
                this.EigenschaftWurdeGeändert();
                Bobbestaetigen.RaiseCanExecuteChanged();
            }
        }
        public string AliasEveText
        {
            get { return _aliaseve; }
            set
            {
                _aliaseve = value;
                this.EigenschaftWurdeGeändert();
                Evebestaetigen.RaiseCanExecuteChanged();
            }
        }
        public string PasswortEveText
        {
            get { return _passworteve; }
            set
            {
                _passworteve = value;
                this.EigenschaftWurdeGeändert();
                Evebestaetigen.RaiseCanExecuteChanged();
            }
        }
        public Visibility AliceBoxesVisible
        {
            get { return _aliceboxesvisible; }
            set
            {
                _aliceboxesvisible = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public Visibility AliceSelected
        {
            get { return _aliceselected; }
            set
            {
                _aliceselected = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public Visibility BobBoxesVisible
        {
            get { return _bobboxesvisible; }
            set
            {
                _bobboxesvisible = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public Visibility BobSelected
        {
            get { return _bobselected; }
            set
            {
                _bobselected = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public Visibility EveBoxesVisible
        {
            get { return _eveboxesvisible; }
            set
            {
                _eveboxesvisible = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        public Visibility EveSelected
        {
            get { return _eveselected; }
            set
            {
                _eveselected = value;
                this.EigenschaftWurdeGeändert();
            }
        }
        private void AliceCommand(object parameter)
        {
            AliceBoxesVisible = Visibility.Collapsed;
            AliceSelected = Visibility.Visible;
            AliasAliceText = "Spieler: " + AliasAliceText;
        }
        private void BobCommand(object parameter)
        {
            BobBoxesVisible = Visibility.Collapsed;
            BobSelected = Visibility.Visible;
            AliasBobText = "Spieler: " + AliasBobText;
        }
        private void EveCommand(object parameter)
        {
            EveBoxesVisible = Visibility.Collapsed;
            EveSelected = Visibility.Visible;
            AliasEveText = "Spieler: " + AliasEveText;
        }
        private void AliceFreigeben(object parameter)
        {
            AliceBoxesVisible = Visibility.Visible;
            AliceSelected = Visibility.Collapsed;
            AliasAliceText = "";
            PasswortAliceText = "";
        }
        private void BobFreigeben(object paramter)
        {
            BobBoxesVisible = Visibility.Visible;
            BobSelected = Visibility.Collapsed;
            AliasBobText = "";
            PasswortBobText = "";
        }
        private void EveFreigeben(object paramter)
        {
            EveBoxesVisible = Visibility.Visible;
            EveSelected = Visibility.Collapsed;
            AliasEveText = "";
            PasswortEveText = "";
        }
    }
}
