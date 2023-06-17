
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using quaKrypto.Commands;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
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
        private string _aliceuebungsszenario = "";
        private string _bobuebungsszenario = "";
        private string _eveuebungsszenario = "";
        private List<Rolle> _eigeneRollen = new List<Rolle>();
        private Visibility _aliceboxesvisible = Visibility.Visible;
        private Visibility _aliceselected = Visibility.Collapsed;
        private Visibility _bobboxesvisible = Visibility.Visible;
        private Visibility _bobselected = Visibility.Collapsed;
        private Visibility _eveboxesvisible = Visibility.Visible;
        private Visibility _eveselected = Visibility.Collapsed;
        private Visibility _evelabel = Visibility.Visible;
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyErstellen { get; set; }
        public DelegateCommand Alicebestaetigen { get; set; }
        public DelegateCommand Bobbestaetigen { get; set; }
        public DelegateCommand Evebestaetigen { get; set; }
        public DelegateCommand ClearAlice { get; set; }
        public DelegateCommand ClearBob { get; set; }
        public DelegateCommand ClearEve { get; set; }
        public List<Rolle> EigeneRollen = new List<Rolle>();
        public LobbyScreenViewModel(Navigator navigator, IUebungsszenario uebungsszenario, bool ishost)
        {
            this.uebungsszenario = uebungsszenario;
            
            ((INotifyCollectionChanged)this.uebungsszenario.Rollen).CollectionChanged += new NotifyCollectionChangedEventHandler(RollenChanged);
            HauptMenu = new((o) =>
            {
                for(int i = 0; i < EigeneRollen.Count; i++)
                {
                    uebungsszenario.GebeRolleFrei(EigeneRollen[i].RolleTyp);
                    
                }
                if (ishost)
                {
                    NetzwerkHost.BeendeTCPLobby();
                }
                else
                {
                    NetzwerkClient.TrenneVerbindungMitUebungsszenario();
                }
                
                
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
            LobbyErstellen = new((o) =>
            {
                uebungsszenario.Starten();
                //uebungsszenario.AktuelleRolle.BeginneZug(_passwortalice);
                SpielEveViewModel eveViewModel = new SpielEveViewModel(navigator, uebungsszenario, EigeneRollen);
                SpielViewModel spielViewModel = new SpielViewModel(navigator, uebungsszenario, EigeneRollen);
                spielViewModel.SpielEveViewModel = eveViewModel;
                eveViewModel.SpielViewModel = spielViewModel;
                
                
                navigator.aktuellesViewModel = spielViewModel;

            }, (o) => ishost && LobbyErstellenStartBedingung());
            Alicebestaetigen = new((o) =>
            {
                
                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(Models.Enums.RolleEnum.Alice, _aliasalice, _passwortalice), false);
                if(success)
                {
                    for(int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Alice)
                        {
                            EigeneRollen.Add(uebungsszenario.Rollen[i]);
                        }
                    }
                    AliceCommand((o));
                }
                else
                {
                    MessageBox.Show("Rolle bereits belegt!", "Rolle vergeben", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passwortalice != "" && _aliasalice != "");
            Bobbestaetigen = new((o) =>
            {
                
                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(Models.Enums.RolleEnum.Bob, _aliasbob, _passwortbob), false);
                if (success)
                {
                    for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Bob)
                        {
                            EigeneRollen.Add(uebungsszenario.Rollen[i]);
                        }
                    }
                    BobCommand((o));
                }
                else
                {
                    MessageBox.Show("Rolle bereits belegt!", "Rolle vergeben", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passwortbob != "" && _aliasbob != "");
            Evebestaetigen = new((o) =>
            {
                
                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(Models.Enums.RolleEnum.Eve, _aliaseve, _passworteve), false);
                if (success)
                {
                    for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Eve)
                        {
                            EigeneRollen.Add(uebungsszenario.Rollen[i]);
                        }
                    }
                    EveCommand((o));
                }
                else
                {
                    MessageBox.Show("Rolle bereits belegt!", "Rolle vergeben", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passworteve != "" && _aliaseve != "");
            ClearAlice = new DelegateCommand(AliceFreigeben);
            ClearBob = new DelegateCommand(BobFreigeben);
            ClearEve = new DelegateCommand(EveFreigeben);
            LobbyName = uebungsszenario.Name;
            if (uebungsszenario.Variante.ToString().Contains("VarianteNormalerAblauf"))
            {
                Variante = VarianteNormalerAblauf.VariantenName;
                EveBoxesVisible = Visibility.Hidden;
                EveSelected = Visibility.Hidden;
                EveLabel = Visibility.Hidden;
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
        public string AliceUebungsszenario
        {
            get
            {
                return _aliceuebungsszenario;                
            }
            set{ _aliceuebungsszenario = "Spieler: " + value; this.EigenschaftWurdeGeändert(); }
        }
        public string BobUebungsszenario
        {
            get
            {
                return _bobuebungsszenario;
            }
            set { _bobuebungsszenario = "Spieler: " + value; this.EigenschaftWurdeGeändert(); }
        }
        public string EveUebungsszenario
        {
            get
            {
                return _eveuebungsszenario;
            }
            set { _eveuebungsszenario = "Spieler: " + value; this.EigenschaftWurdeGeändert(); }
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
        public Visibility EveLabel
        {
            get { return _evelabel; }
            set
            {
                _evelabel = value;
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
            uebungsszenario.GebeRolleFrei(Models.Enums.RolleEnum.Alice);
            for(int i = 0; i < EigeneRollen.Count; i++)
            {
                if (EigeneRollen[i].RolleTyp == Models.Enums.RolleEnum.Alice)
                {
                    EigeneRollen.RemoveAt(i);
                }
            }
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        private void BobFreigeben(object paramter)
        {
            BobBoxesVisible = Visibility.Visible;
            BobSelected = Visibility.Collapsed;
            AliasBobText = "";
            PasswortBobText = "";
            uebungsszenario.GebeRolleFrei(Models.Enums.RolleEnum.Bob);
            for (int i = 0; i < EigeneRollen.Count; i++)
            {
                if (EigeneRollen[i].RolleTyp == Models.Enums.RolleEnum.Bob)
                {
                    EigeneRollen.RemoveAt(i);
                }
            }
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        private void EveFreigeben(object paramter)
        {
            EveBoxesVisible = Visibility.Visible;
            EveSelected = Visibility.Collapsed;
            AliasEveText = "";
            PasswortEveText = "";
            uebungsszenario.GebeRolleFrei(Models.Enums.RolleEnum.Eve);
            for (int i = 0; i < EigeneRollen.Count; i++)
            {
                if (EigeneRollen[i].RolleTyp == Models.Enums.RolleEnum.Eve)
                {
                    EigeneRollen.RemoveAt(i);
                }
            }
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        private void RollenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool alice = false;
            bool bob = false;
            bool eve = false;
            for(int i = 0; i < uebungsszenario.Rollen.Count;i++)
            {
                if (uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Alice)
                {
                    AliceUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    alice = true;
                    AliceBoxesVisible = Visibility.Collapsed;
                    AliceSelected = Visibility.Visible;
                    
                }
                else if(uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Bob)
                {
                    BobUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    bob = true;
                    BobBoxesVisible = Visibility.Collapsed;
                    BobSelected = Visibility.Visible;
                }
                else if (uebungsszenario.Rollen[i].RolleTyp == Models.Enums.RolleEnum.Eve)
                {
                    EveUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    eve = true;
                    EveBoxesVisible = Visibility.Collapsed;
                    EveSelected = Visibility.Visible;
                }

            }
            if(!alice) 
            {
                AliceUebungsszenario = String.Empty;
                AliceBoxesVisible = Visibility.Visible;
                AliceSelected = Visibility.Collapsed;
            }
            if (!bob)
            {
                BobUebungsszenario = String.Empty;
                BobBoxesVisible = Visibility.Visible;
                BobSelected = Visibility.Collapsed;
            }
            if(!eve)
            {
                EveUebungsszenario = String.Empty;
                if (Variante != VarianteNormalerAblauf.VariantenName)
                {
                    EveBoxesVisible = Visibility.Visible;
                    EveSelected = Visibility.Collapsed;
                }
            }
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        private bool LobbyErstellenStartBedingung()
        {
            IList<RolleEnum> benötigteRollen = uebungsszenario.Variante.MoeglicheRollen;
            foreach(RolleEnum rolle in benötigteRollen)
            {
                Rolle? gefunden = uebungsszenario.Rollen.Where(r => r.RolleTyp == rolle).FirstOrDefault();
                if (gefunden == null || gefunden == default(Rolle)) return false;
            }
            return true;
        }
    }
}
