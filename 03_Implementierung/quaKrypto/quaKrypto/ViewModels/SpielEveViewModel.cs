using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class SpielEveViewModel : SpielViewModelBase
    {
        private Navigator navigator;

        private SpielViewModel spielViewModel;
        private bool once = false;
        public SpielViewModel SpielViewModel
        {
            set
            {
                if (!once)
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
        
        public DelegateCommand ZugBeenden { get; set; }
        public SpielEveViewModel(Navigator navigator, IUebungsszenario uebungsszenario, List<Rolle> eigeneRollen) : base(navigator, uebungsszenario, eigeneRollen)
        {
            setzeAktPhaseView();
            setzeAktRolleView();
            this.BituebertragungEingangAlice = new ObservableCollection<Information>();
            this.BituebertragungEingangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangBob = new ObservableCollection<Information>();


            this.BituebertragungAusgangAlice = new ObservableCollection<Information>();
            this.BituebertragungAusgangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangBob = new ObservableCollection<Information>();

            BituebertragungAusgangAlice.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            PhotonenuebertragungAusgangAlice.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            BituebertragungAusgangBob.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            PhotonenuebertragungAusgangBob.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);

            uebungsszenario.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UebungsszenarioChanged);
            //Informationsablage = new ObservableCollection<Information>();
            this.navigator = navigator;

            ZugBeenden = new((o) =>
            {
                AenderZustand(Enums.SpielEnum.warten);
                zugBeenden();
                setzeAktPhaseView();
            }, (o) => ZugBeendenStartBedingung());

            PasswortEingabe = new((o) =>
            {
                if (uebungsszenario.GebeBildschirmFrei(passwortFeld))
                {
                    InformationenLaden();
                    AenderZustand(Enums.SpielEnum.aktiv);
                }
                else
                {
                    //Ungültiges Passwort was tun? -AD
                    //Programm Beenden und Löschen und Win32 löschen -DM
                }

            }, (o) => passwortFeld != "");
        }
        private void UebungsszenarioChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if(e != null)
            {
                if (e.PropertyName is "aktuelleRolle")
                {
                    if (eigeneRollen.Contains(uebungsszenario.AktuelleRolle))
                    {
                        //Prüfen ob Eve dran ist
                        if (uebungsszenario.AktuelleRolle.RolleTyp != RolleEnum.Eve)
                        {
                            //ViewModelWechseln
                            navigator.aktuellesViewModel = spielViewModel;
                        }
                        else
                        {
                            //Visibility ändern
                            AenderZustand(Enums.SpielEnum.passwortEingabe);
                        }
                    }
                    else
                    {
                        //Wartescreen
                        AenderZustand(Enums.SpielEnum.warten);
                    }
                    setzeAktRolleView();
                }
                else if (e.PropertyName is "Beendet") 
                { 
                    if (uebungsszenario.Beendet) HauptMenu.Execute(null); 
                }
            }
            
            
        }

        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanExecuteZugBeenden();
        }

        private void CanExecuteZugBeenden()
        {
            ZugBeenden.RaiseCanExecuteChanged();
        }
        private void zugBeenden()
        {
            //Nachrichten Senden
            NachrichtenSenden();
            //MülltonneLeeren
            for (int i = 0; i < Muelleimer.Count; i++)
            {
                uebungsszenario.LoescheInformation(Muelleimer[i].InformationsID);
            }

            //Informationen aus den Operanden abspeichern
            OperandenInAblageLegen();

            //Informationsablage abspeichern
            for (int i = 0; i < Informationsablage.Count; i++)
            {
                uebungsszenario.SpeichereInformationenAb(Informationsablage[i]);
            }

            //leeren aller Listen für die View
            ClearViewListen();

            //leeren aller TextBoxen
            ClearViewTextBox();

            uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.zugBeenden,
                                    null,
                                    null,
                                    null,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            uebungsszenario.NaechsterZug();
        }
        private void NachrichtenSenden()
        {
            RolleEnum empf = RolleEnum.Alice;
            Information info = new Information(-1, "AutomatischeAngabe", InformationsEnum.keinInhalt, empf, null);
            for (int i = 0; i < BituebertragungAusgangAlice.Count; i++)
            {
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    BituebertragungAusgangAlice[i],
                                    info,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            for (int i = 0; i < PhotonenuebertragungAusgangAlice.Count; i++)
            {

                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    PhotonenuebertragungAusgangAlice[i],
                                    info,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }

            RolleEnum empbob = RolleEnum.Bob;
            Information infobob = new Information(-1, "AutomatischeAngabe", InformationsEnum.keinInhalt, empbob, null);
            for (int i = 0; i < BituebertragungAusgangBob.Count; i++)
            {
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    BituebertragungAusgangBob[i],
                                    infobob,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            for (int i = 0; i < PhotonenuebertragungAusgangBob.Count; i++)
            {

                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    PhotonenuebertragungAusgangBob[i],
                                    infobob,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
        }

        private void OperandenInAblageLegen()
        {
            for (int i = 0; i < Operand1.Count; i++)
            {
                Informationsablage.Add(Operand1[i]);
            }
            for (int i = 0; i < Operand2.Count; i++)
            {
                Informationsablage.Add(Operand2[i]);
            }
            for (int i = 0; i < Ergebnis.Count; i++)
            {
                Informationsablage.Add(Ergebnis[i]);
            }
            for (int i = 0; i < BituebertragungAusgangAlice.Count; i++)
            {
                Informationsablage.Add(BituebertragungAusgangAlice[i]);
            }
            for (int i = 0; i < PhotonenuebertragungAusgangAlice.Count; i++)
            {
                Informationsablage.Add(PhotonenuebertragungAusgangAlice[i]);
            }
            for (int i = 0; i < BituebertragungAusgangBob.Count; i++)
            {
                Informationsablage.Add(BituebertragungAusgangBob[i]);
            }
            for (int i = 0; i < PhotonenuebertragungAusgangBob.Count; i++)
            {
                Informationsablage.Add(PhotonenuebertragungAusgangBob[i]);
            }
        }

        private void ClearViewListen()
        {
            Informationsablage.Clear();
            Muelleimer.Clear();
            Operand1.Clear();
            Operand2.Clear();
            Ergebnis.Clear();
            BituebertragungEingangAlice.Clear();
            PhotonenuebertragungEingangAlice.Clear();
            BituebertragungAusgangAlice.Clear();
            PhotonenuebertragungAusgangAlice.Clear();
            BituebertragungEingangBob.Clear();
            PhotonenuebertragungEingangBob.Clear();
            BituebertragungAusgangBob.Clear();
            PhotonenuebertragungAusgangBob.Clear();
        }

        private bool ZugBeendenStartBedingung()
        {
            for (int i = 0; i < BituebertragungAusgangAlice.Count; i++)
            {
                if (BituebertragungAusgangAlice[i] == null) return false;
                if (BituebertragungAusgangAlice[i].InformationsTyp == InformationsEnum.photonen
                    || BituebertragungAusgangAlice[i].InformationsTyp == InformationsEnum.unscharfePhotonen) return false;
            }

            for (int i = 0; i < PhotonenuebertragungAusgangAlice.Count; i++)
            {
                if (PhotonenuebertragungAusgangAlice[i] == null) return false;
                if (PhotonenuebertragungAusgangAlice[i].InformationsTyp != InformationsEnum.photonen) return false;
            }

            for (int i = 0; i < BituebertragungAusgangBob.Count; i++)
            {
                if (BituebertragungAusgangBob[i] == null) return false;
                if (BituebertragungAusgangBob[i].InformationsTyp == InformationsEnum.photonen
                    || BituebertragungAusgangBob[i].InformationsTyp == InformationsEnum.unscharfePhotonen) return false;
            }

            for (int i = 0; i < PhotonenuebertragungAusgangBob.Count; i++)
            {
                if (PhotonenuebertragungAusgangBob[i] == null) return false;
                if (PhotonenuebertragungAusgangBob[i].InformationsTyp != InformationsEnum.photonen) return false;
            }
            return true;
        }

        private void InformationenLaden()
        {
            for (int i = 0; i < uebungsszenario.AktuelleRolle.Informationsablage.Count; i++)
            {
                Informationsablage.Add(uebungsszenario.AktuelleRolle.Informationsablage[i]);
            }
            InformationenEmpfangen();
            InformationenLöschen();
        }

        //Lädt die Informationen aus den Übertragungskanälen ein
        private void InformationenEmpfangen()
        {
            //Nachrichten Empfangen Handlungsschritte durchführen
            for (int i = 0; i < uebungsszenario.Uebertragungskanal.BitKanal.Count; i++)
            {
                Information empfinfo = uebungsszenario.Uebertragungskanal.BitKanal[i];
                if(empfinfo.InformationsSender != uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    if (empfinfo.InformationsSender == RolleEnum.Alice)
                        BituebertragungEingangAlice.Add(empfinfo);
                    else if (empfinfo.InformationsSender == RolleEnum.Bob)
                        BituebertragungEingangBob.Add(empfinfo);
                }
                
            }
            //Nachrichten Empfangen Handlungsschritte durchführen
            for (int i = 0; i < uebungsszenario.Uebertragungskanal.PhotonenKanal.Count; i++)
            {
                Information empfinfo = uebungsszenario.Uebertragungskanal.PhotonenKanal[i];
                if(empfinfo.InformationsSender != uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    if (empfinfo.InformationsEmpfaenger == RolleEnum.Alice)
                        PhotonenuebertragungEingangAlice.Add(empfinfo);
                    else if (empfinfo.InformationsEmpfaenger == RolleEnum.Bob)
                        PhotonenuebertragungEingangBob.Add(empfinfo);
                }
            }
        }

        //Löscht die Empfangenen Informationen aus den Übertragungskanälen
        private void InformationenLöschen()
        {
            //Löschen der Empfangenen Nachrichten aus dem Übertragungskanälen
            foreach (Information info in BituebertragungEingangAlice)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.bitKanal, info.InformationsID);
            }
            foreach (Information info in BituebertragungEingangBob)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.bitKanal, info.InformationsID);
            }

            foreach (Information info in PhotonenuebertragungEingangAlice)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.photonenKanal, info.InformationsID);
            }
            foreach (Information info in PhotonenuebertragungEingangBob)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.photonenKanal, info.InformationsID);
            }
        }
    }
}
