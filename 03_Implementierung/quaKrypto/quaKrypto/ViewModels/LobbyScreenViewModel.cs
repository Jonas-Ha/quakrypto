﻿
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
        //Visibiltys der LobbySteuerelemente
        private Visibility _aliceboxesvisible = Visibility.Visible;
        private Visibility _aliceselected = Visibility.Collapsed;
        private Visibility _bobboxesvisible = Visibility.Visible;
        private Visibility _bobselected = Visibility.Collapsed;
        private Visibility _eveboxesvisible = Visibility.Collapsed;
        private Visibility _eveselected = Visibility.Collapsed;
        private Visibility _evelabel = Visibility.Collapsed;
        DependencyPropertyChangedEventHandler UebungszenarioStarten;
        //Commands für Hauptmenu, Lobbyerstellen(Spielstarten), und die Rollenbelegen Commands
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

            Wiki.Schwierigkeitsgrad = uebungsszenario.Schwierigkeitsgrad;
            //Fügt einen EventHanlder für das PropertyChanged event hinzu
            uebungsszenario.PropertyChanged += new((o, a) =>
            {
                if (uebungsszenario.HostHatGestartet)
                {
                    SpielEveViewModel eveViewModel = new SpielEveViewModel(navigator, uebungsszenario, EigeneRollen);
                    SpielViewModel spielViewModel = new SpielViewModel(navigator, uebungsszenario, EigeneRollen);
                    spielViewModel.SpielEveViewModel = eveViewModel;
                    eveViewModel.SpielViewModel = spielViewModel;
                    navigator.aktuellesViewModel = spielViewModel;
                }
            });

            ((INotifyCollectionChanged)this.uebungsszenario.Rollen).CollectionChanged += new NotifyCollectionChangedEventHandler(RollenChanged);
            //Connection zum Spiel beendet, Handler wird entfernt
            void handler(object? sender, NotifyCollectionChangedEventArgs e)
            {
                NetzwerkClient.ErrorCollection.CollectionChanged -= handler;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    HauptMenu?.Execute(new string("Connection Closed by the Host"));
                }
            }
            //Wenn der User kein Host ist, wird der Handler hinzugefügt
            if (!ishost) NetzwerkClient.ErrorCollection.CollectionChanged += handler;

            HauptMenu = new((o) =>
            {
                for (int i = 0; i < EigeneRollen.Count; i++)
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


                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator, o.GetType() == typeof(string) ? (string)o : null);

            }, null);

            LobbyErstellen = new((o) =>
            {
                uebungsszenario.Starten();
                SpielEveViewModel eveViewModel = new SpielEveViewModel(navigator, uebungsszenario, EigeneRollen);
                SpielViewModel spielViewModel = new SpielViewModel(navigator, uebungsszenario, EigeneRollen);
                spielViewModel.SpielEveViewModel = eveViewModel;
                eveViewModel.SpielViewModel = spielViewModel;


                navigator.aktuellesViewModel = spielViewModel;

            }, (o) => ishost && LobbyErstellenStartBedingung());

            ClearAlice = new((o) =>
            {
                AliceFreigeben();
            }, (o) => AliceFreigebenStartBedingung());

            ClearBob = new((o) =>
            {
                BobFreigeben();
            }, (o) => BobFreigebenStartBedingung());

            ClearEve = new((o) =>
            {
                EveFreigeben();
            }, (o) => EveFreigebenStartBedingung());

            Alicebestaetigen = new((o) =>
            {

                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(RolleEnum.Alice, _aliasalice, _passwortalice), true);
                if (success)
                {
                    for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == RolleEnum.Alice)
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
                ClearAlice.RaiseCanExecuteChanged();
                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passwortalice != "" && _aliasalice != "");
            Bobbestaetigen = new((o) =>
            {

                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(RolleEnum.Bob, _aliasbob, _passwortbob), true);
                if (success)
                {
                    for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == RolleEnum.Bob)
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
                ClearBob.RaiseCanExecuteChanged();
                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passwortbob != "" && _aliasbob != "");
            Evebestaetigen = new((o) =>
            {

                bool success = uebungsszenario.RolleHinzufuegen(new Rolle(RolleEnum.Eve, _aliaseve, _passworteve), true);
                if (success)
                {
                    for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
                    {
                        if (uebungsszenario.Rollen[i].RolleTyp == RolleEnum.Eve)
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
                ClearEve.RaiseCanExecuteChanged();
                LobbyErstellen.RaiseCanExecuteChanged();
            }, (o) => _passworteve != "" && _aliaseve != "");



            LobbyName = uebungsszenario.Name;
            if (uebungsszenario.Variante.ToString().Contains("VarianteNormalerAblauf"))
            {
                Variante = VarianteNormalerAblauf.VariantenName;
                EveBoxesVisible = Visibility.Hidden;
                EveSelected = Visibility.Hidden;
                EveLabel = Visibility.Hidden;
            }
            else if (uebungsszenario.Variante.ToString().Contains("VarianteAbhören"))
            {
                Variante = VarianteAbhören.VariantenName;
                EveBoxesVisible = Visibility.Visible;
                EveSelected = Visibility.Collapsed;
                EveLabel = Visibility.Visible;
            }
            else
            {
                Variante = VarianteManInTheMiddle.VariantenName;
                EveBoxesVisible = Visibility.Visible;
                EveSelected = Visibility.Collapsed;
                EveLabel = Visibility.Visible;
            }
            //Variante = uebungsszenario.Variante.ToString();
            Schwierigkeit = uebungsszenario.Schwierigkeitsgrad.ToString();
            Phase = uebungsszenario.StartPhase.ToString() + " - " + uebungsszenario.EndPhase.ToString();
            Protokoll = "BB84";

        }
        //Propertys zur Darstellung der Viewinformationen
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
            set { _aliceuebungsszenario = value; this.EigenschaftWurdeGeändert(); }
        }
        public string BobUebungsszenario
        {
            get
            {
                return _bobuebungsszenario;
            }
            set { _bobuebungsszenario = value; this.EigenschaftWurdeGeändert(); }
        }
        public string EveUebungsszenario
        {
            get
            {
                return _eveuebungsszenario;
            }
            set { _eveuebungsszenario = value; this.EigenschaftWurdeGeändert(); }
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
            AliasAliceText = AliasAliceText;
        }
        private void BobCommand(object parameter)
        {
            BobBoxesVisible = Visibility.Collapsed;
            BobSelected = Visibility.Visible;
            AliasBobText = AliasBobText;
        }
        private void EveCommand(object parameter)
        {
            EveBoxesVisible = Visibility.Collapsed;
            EveSelected = Visibility.Visible;
            AliasEveText = AliasEveText;
        }
        //Alice wird freigegeben
        private void AliceFreigeben()
        {
            AliceBoxesVisible = Visibility.Visible;
            AliceSelected = Visibility.Collapsed;
            AliasAliceText = "";
            PasswortAliceText = "";
            uebungsszenario.GebeRolleFrei(Models.Enums.RolleEnum.Alice);
            for (int i = 0; i < EigeneRollen.Count; i++)
            {
                if (EigeneRollen[i].RolleTyp == Models.Enums.RolleEnum.Alice)
                {
                    EigeneRollen.RemoveAt(i);
                }
            }
            ClearAlice.RaiseCanExecuteChanged();
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        //Darf Alice freigegeben werden?
        private bool AliceFreigebenStartBedingung()
        {
            Rolle? gefunden = EigeneRollen.Where(r => r.RolleTyp == RolleEnum.Alice).FirstOrDefault();
            if (gefunden == null || gefunden == default(Rolle)) return false;
            return true;
        }
        //Bob wird freigegeben
        private void BobFreigeben()
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
            ClearBob.RaiseCanExecuteChanged();
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        //Darf Bob freigegeben werden?
        private bool BobFreigebenStartBedingung()
        {
            Rolle? gefunden = EigeneRollen.Where(r => r.RolleTyp == RolleEnum.Bob).FirstOrDefault();
            if (gefunden == null || gefunden == default(Rolle)) return false;
            return true;
        }
        //Eve wird freigegeben
        private void EveFreigeben()
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
            ClearEve.RaiseCanExecuteChanged();
            LobbyErstellen.RaiseCanExecuteChanged();
        }
        //Darf Eve freigegeben werden?
        //Wird hier geprüft
        private bool EveFreigebenStartBedingung()
        {
            Rolle? gefunden = EigeneRollen.Where(r => r.RolleTyp == RolleEnum.Eve).FirstOrDefault();
            if (gefunden == null || gefunden == default(Rolle)) return false;
            return true;
        }
        //Funktion wird aufgerufen wenn sich eine Rolle geändert hat
        private void RollenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool alice = false;
            bool bob = false;
            bool eve = false;
            for (int i = 0; i < uebungsszenario.Rollen.Count; i++)
            {
                if (uebungsszenario.Rollen[i]?.RolleTyp == RolleEnum.Alice)
                {
                    AliceUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    alice = true;
                    AliceBoxesVisible = Visibility.Collapsed;
                    AliceSelected = Visibility.Visible;
                }
                else if (uebungsszenario.Rollen[i]?.RolleTyp == RolleEnum.Bob)
                {
                    BobUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    bob = true;
                    BobBoxesVisible = Visibility.Collapsed;
                    BobSelected = Visibility.Visible;
                }
                else if (uebungsszenario.Rollen[i]?.RolleTyp == RolleEnum.Eve)
                {
                    EveUebungsszenario = uebungsszenario.Rollen[i].Alias;
                    eve = true;
                    EveBoxesVisible = Visibility.Collapsed;
                    EveSelected = Visibility.Visible;
                }
            }

            if (!alice)
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
            if (!eve)
            {

                EveUebungsszenario = String.Empty;
                if (Variante != VarianteNormalerAblauf.VariantenName)
                {
                    EveBoxesVisible = Visibility.Visible;
                    EveSelected = Visibility.Collapsed;
                }
                else
                {
                    EveBoxesVisible = Visibility.Hidden;
                    EveSelected = Visibility.Hidden;
                }

            }
            Application.Current?.Dispatcher.Invoke(new Action(() => LobbyErstellen.RaiseCanExecuteChanged()));

        }
        //Startbedingung um die Lobby zum Starten
        private bool LobbyErstellenStartBedingung()
        {
            IList<RolleEnum> benötigteRollen = uebungsszenario.Variante.MöglicheRollen;
            foreach (RolleEnum rolle in benötigteRollen)
            {
                Rolle? gefunden = uebungsszenario.Rollen.Where(r => r.RolleTyp == rolle).FirstOrDefault();
                if (gefunden == null || gefunden == default(Rolle)) return false;
            }
            if (EigeneRollen.Count == 0) return false;
            return true;
        }

        public string Ueberschrift => $"Lobby \"{_lobbyname}\"";
    }
}
