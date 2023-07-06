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
using System.Windows;

namespace quaKrypto.ViewModels
{
    public class SpielEveViewModel : SpielViewModelBase
    {
        //Um zwischen den Views zu switchen wird der Navigator benötigt
        private Navigator navigator;

        //Wird benötigt damit ein Zugwechsel von Eve nach Alice und Eve nach Bob geschehen kann
        private SpielViewModel spielViewModel;
        private bool once = false;//Einmaliges setzen der SpielViewModel Klasse
        public SpielViewModel SpielViewModel
        {
            set
            {
                //Einmaliges setzen der SpielViewModel Klasse
                if (!once)
                {
                    spielViewModel = value;
                    once = true;
                }
            }
        }

        //Collections für die Übertragungskanäle in der View
        //Die Eingänge enthalten die Nachrichten die an die jeweilige Rolle gerichtet sind
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
            //View aktualisieren
            setzeAktPhaseView();
            setzeAktRolleView();

            //Collections anlegen
            this.BituebertragungEingangAlice = new ObservableCollection<Information>();
            this.BituebertragungEingangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungEingangBob = new ObservableCollection<Information>();


            this.BituebertragungAusgangAlice = new ObservableCollection<Information>();
            this.BituebertragungAusgangBob = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangAlice = new ObservableCollection<Information>();
            this.PhotonenuebertragungAusgangBob = new ObservableCollection<Information>();

            //Events festlegen falls der Benutzer Informationen in den Ausgang legt/entfernt
            BituebertragungAusgangAlice.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            PhotonenuebertragungAusgangAlice.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            BituebertragungAusgangBob.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            PhotonenuebertragungAusgangBob.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedMethod);

            //Event festlegen, falls sich etwas im Übungsszenario ändert
            uebungsszenario.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UebungsszenarioChanged);

            //Navigator abspeichern
            this.navigator = navigator;

            //Command wenn der Benutzer auf Hauptmenü klickt
            HauptMenu = new((o) =>
            {
                //Falls die Rolle freigeschaltet war wird noch ein Zugbeenden ausgeführt,
                //ohne das dabei die Informationen im Ausgang gesendet werden
                if (uebungsszenario.AktuelleRolle.Freigeschaltet) zugBeendenOhneSenden();

                //Ablauf zum Beenden des Übungsszenario einleiten
                uebungsszenario.Beenden();

                //In das AufzeichnungsViewModel wechseln
                Application.Current.Dispatcher.Invoke(() => { navigator.aktuellesViewModel = new AufzeichnungViewModel(navigator, uebungsszenario); });
            }, (o) => true);

            //Command wenn der Benutzer den Zug beendet
            ZugBeenden = new((o) =>
            {
                //Ansicht auf den Warte Screen setzten
                AenderZustand(Enums.SpielEnum.warten);

                //Ablauf zum Zugbeenden starten
                zugBeenden();

                //Ansicht der aktuellen Phase neusetzen
                setzeAktPhaseView();
            }, (o) => ZugBeendenStartBedingung());//Prüfen der Startbedingung zum Zugbeenden

            //Command wenn der Benutzer ein Passwort zum Freischalten der Rolle eingibt
            PasswortEingabe = new((o) =>
            {
                //Anfrage an das Übungsszenario mit dem Passwort geben, damit die Rolle freigegeben wird
                if (uebungsszenario.GebeBildschirmFrei(passwortFeld))
                {
                    //Laden der Informationen in die Ablage und in die Eingänge
                    InformationenLaden();
                    //Ändern der Ansicht auf das Aktive Spielfeld
                    AenderZustand(Enums.SpielEnum.aktiv);
                }
            }, (o) => passwortFeld != "");//Passwort darf nicht leer sein
        }

        //Wird ausgelöst wenn sich etwas im Übungsszenario geändert hat 
        private void UebungsszenarioChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e != null)
            {
                //Prüfen ob sich die aktuelleRolle geändert hat
                if (e.PropertyName is "aktuelleRolle")
                {
                    //Prüfen ob eine der EigenenRollen die der Benutzer in der Lobby ausgewählt hat betroffen
                    if (eigeneRollen.Contains(uebungsszenario.AktuelleRolle))
                    {
                        //Prüfen ob Eve nicht dran ist
                        if (uebungsszenario.AktuelleRolle.RolleTyp != RolleEnum.Eve)
                        {
                            //ViewModelWechseln in das SpielViewModel
                            navigator.aktuellesViewModel = spielViewModel;
                        }
                        else
                        {
                            //Visibility ändern Passwortfenster aktivieren
                            AenderZustand(Enums.SpielEnum.passwortEingabe);
                        }
                    }
                    else
                    {
                        //Zum Wartescreen wechseln
                        AenderZustand(Enums.SpielEnum.warten);
                    }
                    //Rollen in der Ansicht aktualisieren
                    setzeAktRolleView();

                    //VerfügbareOperationen aktualiseren
                    verfügbareOperationen = uebungsszenario.Variante.GebeHilfestellung(uebungsszenario.Schwierigkeitsgrad);
                    //Operanden in die Informationsablage legen
                    OperandenInAblageLegen();
                    //Aktualisieren der Ansicht der verfügbaren Operationen
                    AktualisiereOperationenVisibility();
                }
                //Prüfen ob das Übungsszenario beendet wurde
                else if (e.PropertyName is "Beendet")
                {
                    //Falls Übungsszenario beendet wurde, Wechsel in die Aufzeichnung starten
                    if (uebungsszenario.Beendet) Beendet.Execute(null);
                }
            }


        }

        //Wird ausgelöst wenn sich etwas im ÜbertragungskanalAusgang ändert
        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanExecuteZugBeenden();
        }

        //Löst Überprüfung zum Zugbeenden aus
        private void CanExecuteZugBeenden()
        {
            ZugBeenden.RaiseCanExecuteChanged();
        }

        //Ablauf der durchgeführt wird wenn der Zugbeendet wurde
        private void zugBeenden()
        {
            //Nachrichten Senden
            NachrichtenSenden();

            //Informationen aus den Operanden abspeichern
            InformationenInAblageLegen();

            //Informationsablage unscharfe Photonen entfernen und zu Muelleimer hinzufuegen
            for (int i = Informationsablage.Count - 1; i >= 0; i--)
            {
                if (Informationsablage[i].InformationsTyp == quaKrypto.Models.Enums.InformationsEnum.unscharfePhotonen)
                {
                    Muelleimer.Add(Informationsablage[i]);
                    Informationsablage.RemoveAt(i);
                }
            }

            //MülltonneLeeren
            for (int i = 0; i < Muelleimer.Count; i++)
            {
                uebungsszenario.LoescheInformation(Muelleimer[i].InformationsID);
            }

            //Informationsablage abspeichern
            for (int i = 0; i < Informationsablage.Count; i++)
            {
                uebungsszenario.SpeichereInformationenAb(Informationsablage[i]);
            }

            //leeren aller Listen für die View
            ClearViewListen();

            //leeren aller TextBoxen
            ClearViewTextBox();

            //Zug beenden Handlungsschritt ausführen
            uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.zugBeenden,
                                    null,
                                    null,
                                    null,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            //Nächsten Zug starten
            uebungsszenario.NaechsterZug();
        }

        //Wird ausgelöst wenn der Zug durch drücken auf Hauptmenü ausgelöst werden
        private void zugBeendenOhneSenden()
        {
            //MülltonneLeeren
            for (int i = 0; i < Muelleimer.Count; i++)
            {
                uebungsszenario.LoescheInformation(Muelleimer[i].InformationsID);
            }

            //Informationen aus den Operanden abspeichern
            InformationenInAblageLegen();

            //Informationsablage abspeichern
            for (int i = 0; i < Informationsablage.Count; i++)
            {
                uebungsszenario.SpeichereInformationenAb(Informationsablage[i]);
            }

            //leeren aller Listen für die View
            ClearViewListen();

            //leeren aller TextBoxen
            ClearViewTextBox();

            //Zug beenden Handlungsschritt ausführen
            uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.zugBeenden,
                                    null,
                                    null,
                                    null,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            //Nächsten Zug starten
            uebungsszenario.NaechsterZug();
        }

        //Nachrichten senden am Ende des Zuges
        private void NachrichtenSenden()
        {
            //Empfänger auf Alice setzen und die Ausgänge die an Alice gerichtet sind an Alice senden
            RolleEnum empf = RolleEnum.Alice;

            //Operand mit Empfänger erzeugen
            Information info = new Information(-1, "AutomatischeAngabe", InformationsEnum.keinInhalt, empf, null);
            for (int i = 0; i < BituebertragungAusgangAlice.Count; i++)
            {
                //Senden der Nachricht 
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
                //Senden der Nachricht 
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    PhotonenuebertragungAusgangAlice[i],
                                    info,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }

            //Empfänger auf Bob setzen und die Ausgänge die an Bob gerichtet sind an Bob senden
            RolleEnum empbob = RolleEnum.Bob;

            //Operand mit Empfänger erzeugen
            Information infobob = new Information(-1, "AutomatischeAngabe", InformationsEnum.keinInhalt, empbob, null);
            for (int i = 0; i < BituebertragungAusgangBob.Count; i++)
            {
                //Senden der Nachricht 
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
                //Senden der Nachricht 
                uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.nachrichtSenden,
                                    PhotonenuebertragungAusgangBob[i],
                                    infobob,
                                    "NachrichtDummy",
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
        }

        //Informationen in die Informationsablage aus den Ablagen und den Craftingfeld ablegen
        private void InformationenInAblageLegen()
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

        //Leeren der ViewListen (Ablage Craftingfeld und Übertragungskanälen)
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

        //Überprüft, ob der Zugbeendet werden kann (mit Nachrichten senden)
        private bool ZugBeendenStartBedingung()
        {
            //Prüft, ob im Bitübertragungskanal Photonen oder unscharfePhotonen liegen
            //und gibt ein false zurück fall dies der Fall ist
            for (int i = 0; i < BituebertragungAusgangAlice.Count; i++)
            {
                if (BituebertragungAusgangAlice[i] == null) return false;
                if (BituebertragungAusgangAlice[i].InformationsTyp == InformationsEnum.photonen
                    || BituebertragungAusgangAlice[i].InformationsTyp == InformationsEnum.unscharfePhotonen) return false;
            }

            //Prüft, ob im Photonenübertragungskanal etwas anderes als Photonen liegen
            //und gibt ein false zurück fall dies der Fall ist
            for (int i = 0; i < PhotonenuebertragungAusgangAlice.Count; i++)
            {
                if (PhotonenuebertragungAusgangAlice[i] == null) return false;
                if (PhotonenuebertragungAusgangAlice[i].InformationsTyp != InformationsEnum.photonen) return false;
            }

            //Prüft, ob im Bitübertragungskanal Photonen oder unscharfePhotonen liegen
            //und gibt ein false zurück fall dies der Fall ist
            for (int i = 0; i < BituebertragungAusgangBob.Count; i++)
            {
                if (BituebertragungAusgangBob[i] == null) return false;
                if (BituebertragungAusgangBob[i].InformationsTyp == InformationsEnum.photonen
                    || BituebertragungAusgangBob[i].InformationsTyp == InformationsEnum.unscharfePhotonen) return false;
            }

            //Prüft, ob im Photonenübertragungskanal etwas anderes als Photonen liegen
            //und gibt ein false zurück fall dies der Fall ist
            for (int i = 0; i < PhotonenuebertragungAusgangBob.Count; i++)
            {
                if (PhotonenuebertragungAusgangBob[i] == null) return false;
                if (PhotonenuebertragungAusgangBob[i].InformationsTyp != InformationsEnum.photonen) return false;
            }
            return true;
        }

        //Laden der Informationen aus der aktuellen Rolle in die Informationablagenliste für die View
        private void InformationenLaden()
        {
            //Informationsablage wiederherstellen
            for (int i = 0; i < uebungsszenario.AktuelleRolle.Informationsablage.Count; i++)
            {
                Informationsablage.Add(uebungsszenario.AktuelleRolle.Informationsablage[i]);
            }

            //Empfangen der Informationen aus dem Übertragungskanal
            InformationenEmpfangen();
            //Löschen der Informationen aus dem Übertragungskanal
            InformationenLöschen();
        }

        //Lädt die Informationen aus den Übertragungskanälen ein
        private void InformationenEmpfangen()
        {
            //Nachrichten Empfangen Handlungsschritte durchführen
            for (int i = 0; i < uebungsszenario.Uebertragungskanal.BitKanal.Count; i++)
            {
                Information empfinfo = uebungsszenario.Uebertragungskanal.BitKanal[i];
                //Prüfen ob die Nachricht von Eve gesendet wurde
                if (empfinfo.InformationsSender != uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    //Prüfen ob die empfangene Information an Alice gerichtet ist
                    if (empfinfo.InformationsSender == RolleEnum.Alice)
                    {
                        BituebertragungEingangAlice.Add(empfinfo);
                        uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                            empfinfo,
                            null,
                            empfinfo.InformationsName,
                            uebungsszenario.AktuelleRolle.RolleTyp);
                    }
                    //Prüfen ob die empfangene Information an Bob gerichtet ist
                    else if (empfinfo.InformationsSender == RolleEnum.Bob)
                    {
                        BituebertragungEingangBob.Add(empfinfo);
                        uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                                empfinfo,
                                null,
                                empfinfo.InformationsName,
                                uebungsszenario.AktuelleRolle.RolleTyp);
                    }
                }
            }
            //Nachrichten Empfangen Handlungsschritte durchführen
            for (int i = 0; i < uebungsszenario.Uebertragungskanal.PhotonenKanal.Count; i++)
            {
                Information empfinfo = uebungsszenario.Uebertragungskanal.PhotonenKanal[i];
                //Prüfen ob die Nachricht von Eve gesendet wurde
                if (empfinfo.InformationsSender != uebungsszenario.AktuelleRolle.RolleTyp)
                {
                    //Prüfen ob die empfangene Information an Alice gerichtet ist
                    if (empfinfo.InformationsEmpfaenger == RolleEnum.Alice)
                    {
                        PhotonenuebertragungEingangAlice.Add(empfinfo);
                        uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                            empfinfo,
                            null,
                            empfinfo.InformationsName,
                            uebungsszenario.AktuelleRolle.RolleTyp);
                    }
                    //Prüfen ob die empfangene Information an Bob gerichtet ist
                    else if (empfinfo.InformationsEmpfaenger == RolleEnum.Bob)
                    {
                        PhotonenuebertragungEingangBob.Add(empfinfo);
                        uebungsszenario.HandlungsschrittAusführenLassen(OperationsEnum.nachrichtEmpfangen,
                            empfinfo,
                            null,
                            empfinfo.InformationsName,
                            uebungsszenario.AktuelleRolle.RolleTyp);
                    }                       
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
