using Microsoft.VisualBasic;
using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Information = quaKrypto.Models.Classes.Information;

namespace quaKrypto.ViewModels
{
    public class SpielViewModel : SpielViewModelBase
    {
        private Navigator navigator;

        private SpielEveViewModel spielEveViewModel;
        private bool once = false;
        public SpielEveViewModel SpielEveViewModel { 
            set
            { 
                if (!once) 
                { 
                    spielEveViewModel = value;
                    once = true;
                } 
            } 
        }
        public ObservableCollection<Information> BituebertragungEingang { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungEingang { get; set; }
        public ObservableCollection<Information> BituebertragungAusgang { get; set; }
        public ObservableCollection<Information> PhotonenuebertragungAusgang { get; set; }

        public DelegateCommand ZugBeenden { get; set; }

        public SpielViewModel(Navigator navigator, IUebungsszenario uebungsszenario, List<Rolle> eigeneRollen) : base(navigator, uebungsszenario, eigeneRollen)
        {
            setzeAktPhaseView();
            setzeAktRolleView();
            this.BituebertragungEingang = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingang = new ObservableCollection<Information>();

            this.BituebertragungAusgang = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgang = new ObservableCollection<Information>();

            BituebertragungAusgang.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            PhotonenuebertragungAusgang.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);

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
            if (uebungsszenario.Beendet) HauptMenu.Execute(null);//navigator.aktuellesViewModel = ;
            if (eigeneRollen.Contains(uebungsszenario.AktuelleRolle))
            {
                //Prüfen ob Eve dran ist
                if(uebungsszenario.AktuelleRolle.RolleTyp == RolleEnum.Eve)
                {
                    //ViewModelWechseln
                    navigator.aktuellesViewModel = spielEveViewModel;
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
            for(int i = 0;i < Informationsablage.Count; i++)
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
            if (uebungsszenario.AktuelleRolle.RolleTyp == RolleEnum.Alice) empf = RolleEnum.Bob;
            else if (uebungsszenario.AktuelleRolle.RolleTyp == RolleEnum.Bob) empf = RolleEnum.Alice;
            Information info = new Information(-1, "AutomatischeAngabe", InformationsEnum.keinInhalt, empf, null);
            for (int i = 0; i<BituebertragungAusgang.Count; i++)
            {
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    BituebertragungAusgang[i],
                                    info,  
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            for (int i = 0; i < PhotonenuebertragungAusgang.Count; i++)
            {
                
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    PhotonenuebertragungAusgang[i],
                                    info,
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
            for (int i = 0; i < BituebertragungAusgang.Count; i++)
            {
                Informationsablage.Add(BituebertragungAusgang[i]);
            }
            for (int i = 0; i < PhotonenuebertragungAusgang.Count; i++)
            {
                Informationsablage.Add(PhotonenuebertragungAusgang[i]);
            }
        }

        private void ClearViewListen()
        {
            Informationsablage.Clear();
            Muelleimer.Clear();
            Operand1.Clear();
            Operand2.Clear();
            Ergebnis.Clear();
            BituebertragungEingang.Clear();
            PhotonenuebertragungEingang.Clear();
            BituebertragungAusgang.Clear();
            PhotonenuebertragungAusgang.Clear();
        }

        private bool ZugBeendenStartBedingung()
        {
            for(int i = 0; i < BituebertragungAusgang.Count; i++)
            {
                if (BituebertragungAusgang[i] == null)return false;
                if (BituebertragungAusgang[i].InformationsTyp == InformationsEnum.photonen 
                    || BituebertragungAusgang[i].InformationsTyp == InformationsEnum.unscharfePhotonen) return false;
            }

            for (int i = 0; i < PhotonenuebertragungAusgang.Count; i++)
            {
                if (PhotonenuebertragungAusgang[i] == null) return false;
                if (PhotonenuebertragungAusgang[i].InformationsTyp != InformationsEnum.photonen) return false;
            }
            return true;
        }

        private void InformationenLaden()
        {
            //Informationsablage wiederherstellen
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
                if (empfinfo.InformationsEmpfaenger == uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                        empfinfo,
                        null,
                        empfinfo.InformationsName,
                        uebungsszenario.AktuelleRolle.RolleTyp);
                    BituebertragungEingang.Add(empfinfo);
                }

            }
            //Nachrichten Empfangen Handlungsschritte durchführen
            for (int i = 0; i < uebungsszenario.Uebertragungskanal.PhotonenKanal.Count; i++)
            {
                Information empfinfo = uebungsszenario.Uebertragungskanal.PhotonenKanal[i];
                if (empfinfo.InformationsEmpfaenger == uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                        empfinfo,
                        null,
                        empfinfo.InformationsName,
                        uebungsszenario.AktuelleRolle.RolleTyp);
                    PhotonenuebertragungEingang.Add(empfinfo);
                }

            }
        }

        //Löscht die Empfangenen Informationen aus den Übertragungskanälen
        private void InformationenLöschen()
        {
            //Löschen der Empfangenen Nachrichten aus dem Übertragungskanälen
            foreach (Information info in BituebertragungEingang)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.bitKanal, info.InformationsID);
            }
            foreach (Information info in PhotonenuebertragungEingang)
            {
                uebungsszenario.LoescheInformationAusUebertragungskanal(KanalEnum.photonenKanal, info.InformationsID);
            }
        }
    }
}
