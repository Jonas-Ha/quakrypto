using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace quaKrypto.ViewModels
{
    public class SpielViewModelBase : BaseViewModel
    {
        protected IUebungsszenario uebungsszenario;

        //Hilfsklasse anlegen um später die Textlänge von Informationen bestimmen zu können
        private Operationen hilfsoperationen = new Operationen();

        //Collections anlegen für Informationsablage und den Mülleimer
        public ObservableCollection<Information> Muelleimer { get; set; }
        public ObservableCollection<Information> Informationsablage { get; set; }

        //Property zum Binden der Ausgewählten Operation
        public int CraftingFeldSelectedOperation { get; set; }

        //Liste der Rollen, die der Benutzer in der Lobby angelegt hat
        protected List<Rolle> eigeneRollen;

        //Visibility Property für die Warteansicht
        private Visibility warteVisibility;
        public Visibility WarteVisibility
        {
            get { return warteVisibility; }
            set { warteVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        //Visibility Property für die Passworteingabeansicht
        private Visibility passwortEingabeVisibility;
        public Visibility PasswortEingabeVisibility
        {
            get { return passwortEingabeVisibility; }
            set { passwortEingabeVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        //Visibility Property für die aktive Spielansicht
        private Visibility spielVisibility;
        public Visibility SpielVisibility
        {
            get { return spielVisibility; }
            set { spielVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        //Property für die Passworteingabe
        private string passwort = string.Empty;
        public string passwortFeld
        {
            get { return passwort; }
            set { passwort = value; this.EigenschaftWurdeGeändert(); this.PasswortEingabe.RaiseCanExecuteChanged(); }
        }

        //Property für den Path der Icons der Rollen
        private string rolleIcon;
        public string RolleIcon
        {
            set
            {
                rolleIcon = value;
                this.EigenschaftWurdeGeändert();
            }
            get
            {
                return rolleIcon;
            }
        }

        //Property für die aktuelle Rolle, die gerade am Zug ist
        private string aktuelleRolleAnzeige;
        public string AktuelleRolleAnzeige
        {
            set
            {
                aktuelleRolleAnzeige = value;
                this.EigenschaftWurdeGeändert();
            }
            get
            {
                return aktuelleRolleAnzeige;
            }
        }

        //Property für die aktuelle Phase, in der sich das Übungsszenario gerade befindet
        private string aktuellePhaseAnzeige;
        public string AktuellePhaseAnzeige
        {
            set
            {
                if (aktuellePhaseAnzeige != value)
                {
                    aktuellePhaseAnzeige = value;
                    this.EigenschaftWurdeGeändert();
                }
            }
            get
            {
                return aktuellePhaseAnzeige;
            }
        }
        //Variablen zum aktualisieren der Verfügbaren Operationen bei einem Phasenwechsel
        private uint vorherigePhase = 0;
        private uint aktuellePhase = 0;

        #region CraftingFeld
        //Property des Names der Zu erzeugenden Information
        private string informationsname;
        public string Informationsname
        {
            get { return informationsname; }
            set { informationsname = value; this.EigenschaftWurdeGeändert(); CanExecute(); }
        }

        //Collection für das Fenster, das den Operanden1 enthält
        private ObservableCollection<Information> operand1;
        public ObservableCollection<Information> Operand1
        {
            get { return operand1; }
            set
            {
                operand1 = value; this.EigenschaftWurdeGeändert();
            }
        }

        //Collection für das Fenster, das den OperandBitsFrei enthält, damit Bitsfrei bearbeitet werden können
        private ObservableCollection<Information> operandBitsFrei;
        public ObservableCollection<Information> OperandBitsFrei
        {
            get { return operandBitsFrei; }
            set
            {
                operandBitsFrei = value; this.EigenschaftWurdeGeändert();
            }
        }

        //Collection für das Fenster, das den Operanden2 enthält
        private ObservableCollection<Information> operand2;
        public ObservableCollection<Information> Operand2
        {
            get { return operand2; }
            set
            {
                operand2 = value; this.EigenschaftWurdeGeändert();
            }
        }

        //Property die anzeigt ob der Benutzer auf manuelle Eingabe im Crafting Feld gedrückt hat
        private bool eingabeBool;
        public bool EingabeBool
        {
            get { return eingabeBool; }
            set
            {
                eingabeBool = value; this.EigenschaftWurdeGeändert(); CanExecute();
            }
        }

        //Property über die der Benutzer manuelle Eingaben tätigen kann
        private string eingabe;
        public string Eingabe
        {
            get { return eingabe; }
            set
            {
                eingabe = value; this.EigenschaftWurdeGeändert(); CanExecute();
            }
        }

        //Collection für das Fenster, die das Ergebnis der Operation enthält
        private ObservableCollection<Information> ergebnis;
        public ObservableCollection<Information> Ergebnis
        {
            get { return ergebnis; }
            set
            {
                ergebnis = value; this.EigenschaftWurdeGeändert();
            }
        }
        #endregion
        #region VisibilityOperationen
        //Liste der Verügbaren Operationen, die am Anfang alle Operationen enthält
        protected List<OperationsEnum> verfügbareOperationen = Enum.GetValues(typeof(OperationsEnum)).Cast<OperationsEnum>().ToList();
        
        //Properties, die Anzeigen, ob eine Operation sichtbar ist
        public Visibility BitfolgeGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                //Operation soll angezeigt werden wenn eine der Operationen vorhanden ist
                return !(verfügbareOperationen.Contains(OperationsEnum.bitfolgeGenerierenZahl) || verfügbareOperationen.Contains(OperationsEnum.bitfolgeGenerierenAngabe)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility PolarisationsschemataGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                //Operation soll angezeigt werden wenn eine der Operationen vorhanden ist
                return !(verfügbareOperationen.Contains(OperationsEnum.polarisationsschemataGenerierenAngabe) || verfügbareOperationen.Contains(OperationsEnum.polarisationsschemataGenerierenZahl)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility PhotonenGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.photonenGenerieren)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility BitMaskeGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.bitmaskeGenerieren)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility VergleichenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                //Operation soll angezeigt werden wenn eine der Operationen vorhanden ist
                return !(verfügbareOperationen.Contains(OperationsEnum.polschataVergleichen)||verfügbareOperationen.Contains(OperationsEnum.bitfolgenVergleichen)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility BitfolgeNegierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.bitfolgeNegieren)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility PhotonenZuBitfolgeVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.photonenZuBitfolge)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility TextGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.textGenerieren)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility TextLaengeBestimmenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.textLaengeBestimmen)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility TextVerEntschlüsselnVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                //Operation soll angezeigt werden wenn eine der Operationen vorhanden ist
                return !(verfügbareOperationen.Contains(OperationsEnum.textEntschluesseln)|| verfügbareOperationen.Contains(OperationsEnum.textVerschluesseln)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility BitsStreichenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.bitsStreichen)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility BitsFreiBearbeitenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.bitsFreiBearbeiten)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility ZahlGenerierenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.zahlGenerieren)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility InformationUmbenennenVisibility
        {
            get
            {
                //Falls der Schwierigkeitsgrad schwer ist soll jede Operation angezeigt werden
                if (uebungsszenario.Schwierigkeitsgrad == SchwierigkeitsgradEnum.Schwer) return Visibility.Visible;
                return !(verfügbareOperationen.Contains(OperationsEnum.informationUmbenennen)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region Commands
        //Command um zurück ins HauptMenü zu wechseln
        public DelegateCommand HauptMenu { get; set; }
        //Command der Ausgelöst wird, wenn das Übungsszenario beendet ist
        public DelegateCommand Beendet { get; set; }
        //Command wenn nach der Passworteingabe auf Bestätigen gedrückt wird
        public DelegateCommand PasswortEingabe { get; set; }

        //Command zum Craftingfeld
        public DelegateCommand OperationenAnzeigen { get; set; }
        public DelegateCommand BitFolgeErzeugen { get; set; }
        public DelegateCommand EntVerschlüsseln { get; set; }
        public DelegateCommand PhotonenErzeugen { get; set; }
        public DelegateCommand PolschaErzeugen { get; set; }
        public DelegateCommand Streichen { get; set; }
        public DelegateCommand Vergleichen { get; set; }
        public DelegateCommand ZahlErzeugen { get; set; }
        public DelegateCommand BitMaskeGenerieren { get; set; }
        public DelegateCommand BitfolgeNegieren { get; set; }
        public DelegateCommand PhotonenZuBitfolge { get; set; }
        public DelegateCommand TextGenerieren { get; set; }
        public DelegateCommand TextLaengeBestimmen { get; set; }
        public DelegateCommand BitsFreiBearbeiten { get; set; }
        public DelegateCommand InformationUmbenennen { get; set; }


        #endregion

        public SpielViewModelBase(Navigator navigator, IUebungsszenario uebungsszenario, List<Rolle> eigeneRollen)
        {
            //Verweis auf Übungsszenario abspeichern
            this.uebungsszenario = uebungsszenario;
            //Eigene Rollen des Benutzers abspeichern
            this.eigeneRollen = eigeneRollen;

            //Aktuelle Verfügbare Operationen laden
            verfügbareOperationen = this.uebungsszenario.Variante.GebeHilfestellung(this.uebungsszenario.Schwierigkeitsgrad);
            //Operationen neu laden
            AktualisiereOperationenVisibility();

            //Aktuelle Phase abspeichern
            aktuellePhase = this.uebungsszenario.Variante.AktuellePhase;
            vorherigePhase = this.uebungsszenario.Variante.AktuellePhase;

            //Prüfen ob eine eigenRolle am Zug ist falls ja wird in die Passworteingabe gewechselt ansonsten in die Warteansicht
            if (this.eigeneRollen.Contains(this.uebungsszenario.AktuelleRolle)) AenderZustand(Enums.SpielEnum.passwortEingabe);
            else AenderZustand(Enums.SpielEnum.warten);

            //Implementierung des Commands wenn das Übungsszenario geendet ist
            Beendet = new((o) =>
            {
                Application.Current.Dispatcher.Invoke(() => { navigator.aktuellesViewModel = new AufzeichnungViewModel(navigator, uebungsszenario); });
            }, (o) => true);

            //Implementierung des Commands wenn der Benutzer auf Alle Operationen Anzeigen klickt
            OperationenAnzeigen = new((o) =>
            {
                verfügbareOperationen = Enum.GetValues(typeof(OperationsEnum)).Cast<OperationsEnum>().ToList();
                AktualisiereOperationenVisibility();
            }, (o) => true);

            #region OperationenCommands
            //Implementierung der Commands für das Craftingfeld
            BitFolgeErzeugen = new((o) =>
            {
                Ergebnis.Add(bitfolgeErzeugen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren(); //Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => bitfolgeErzeugenStartBedingung()); //Startbedingung für Operation prüfen

            EntVerschlüsseln = new((o) =>
            {
                Ergebnis.Add(entVerschlüsseln());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => entVerschlüsselnStartBedingung());//Startbedingung für Operation prüfen

            PhotonenErzeugen = new((o) =>
            {
                Ergebnis.Add(photonenErzeugen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => photonenErzeugenStartBedingung());//Startbedingung für Operation prüfen

            PolschaErzeugen = new((o) =>
            {
                Ergebnis.Add(polschaErzeugen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => polschaErzeugenStartBedingung());//Startbedingung für Operation prüfen

            Streichen = new((o) =>
            {
                Ergebnis.Add(streichen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => streichenStartBedingung());//Startbedingung für Operation prüfen

            Vergleichen = new((o) =>
            {
                Ergebnis.Add(vergleichen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => vergleichenStartBedingung());//Startbedingung für Operation prüfen

            ZahlErzeugen = new((o) =>
            {
                Ergebnis.Add(zahlErzeugen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => zahlErzeugenStartBedingung());//Startbedingung für Operation prüfen

            BitMaskeGenerieren = new((o) =>
            {
                Ergebnis.Add(bitMaskeGenerieren());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => bitMaskeGenerierenStartBedingung());

            BitfolgeNegieren = new((o) =>
            {
                Ergebnis.Add(bitfolgeNegieren());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => bitfolgeNegierenStartBedingung());//Startbedingung für Operation prüfen

            PhotonenZuBitfolge = new((o) =>
            {
                Ergebnis.Add(photonenZuBitfolge());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
                //Löschen der Information aus Operand1 damit der Benutzer dies Information nicht duplizieren kann
                Operand1.Clear(); 
            }, (o) => photonenZuBitfolgeStartBedingung());//Startbedingung für Operation prüfen

            TextGenerieren = new((o) =>
            {
                Ergebnis.Add(textGenerieren());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => textGenerierenStartBedingung());//Startbedingung für Operation prüfen

            TextLaengeBestimmen = new((o) =>
            {
                Ergebnis.Add(textLaengeBestimmen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => textLaengeBestimmenStartBedingung());//Startbedingung für Operation prüfen

            BitsFreiBearbeiten = new((o) =>
            {
                Ergebnis.Add(bitsFreiBearbeiten());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
            }, (o) => bitsFreiBearbeitenStartBedingung());//Startbedingung für Operation prüfen

            InformationUmbenennen = new((o) =>
            {
                Ergebnis.Add(informationUmbenennen());//Ergebnis berechnen und abspeichern
                PhaseAktualisieren();//Ablauf starten, der prüft ob sich die Phase aktualisiert hat
                //Löschen der Information aus Operand1 damit der Benutzer dies Information nicht duplizieren kann
                Operand1.Clear();
            }, (o) => informationUmbenennenStartBedingung());//Startbedingung für Operation prüfen

            #endregion

            //Collections für Craftingfeld anlegen
            Operand1 = new ObservableCollection<Information>();
            Operand2 = new ObservableCollection<Information>();
            OperandBitsFrei = new ObservableCollection<Information>();
            Ergebnis = new ObservableCollection<Information>();

            //Events auf Funktionen mappen
            Operand1.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            Operand2.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            OperandBitsFrei.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethodBitsFrei);

            //Collection für Mülleimer anlegen
            Muelleimer = new ObservableCollection<Information>();
            //Collection für Informationsablage anlegen
            Informationsablage = new ObservableCollection<Information>();

            //Angeben welche Funktion ausgelöst werden soll, wenn sich etwas in der Variante ändert
            uebungsszenario.Variante.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VarianteChanged);
        }

        //Ablauf wenn sich die Variante geändert hat
        private void VarianteChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Informationen für die View anpassen
            setzeAktPhaseView();
            //Aktualisieren der Aktuellen Phase
            aktuellePhase = uebungsszenario.Variante.AktuellePhase;
        }

        //Ablauf nachdem eine Operation ausgeführt wurde, die das Craftingfeld zurücksetzt wenn sich die Phase geändert hat
        private void PhaseAktualisieren()
        {
            //Prüfen ob sich die Phase nach der Operation geändert hat
            if (aktuellePhase > vorherigePhase)
            {
                //Operanden und Ergebnis in die Informationsablage abglegen
                OperandenInAblageLegen();
                //Liste der verfügbaren Operationen von der Variante bekommen
                verfügbareOperationen = uebungsszenario.Variante.GebeHilfestellung(uebungsszenario.Schwierigkeitsgrad);
                //Crafting feld aktualisieren und verfügbare Operationen anzeigen
                AktualisiereOperationenVisibility();
                //Abspeichern der aktuellenPhase
                vorherigePhase = aktuellePhase;
            }
        }

        //Operanden und Ergebnis in die Informationsablage legen und leeren
        protected void OperandenInAblageLegen()
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

            //Leeren der Operanden und des Ergbnisses
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Operand1.Clear();
                Operand2.Clear();
                Ergebnis.Clear();
            }));
            
        }

        //Wird ausgelöst, wenn der Benutzer etwas in die Operanden legt
        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Löst die Abfrage aus welche Operaden ausgeführt werden können
            CanExecute();
        }

        //Wird ausgelöst, wenn eine Information in den Operand OperandBitsFrei legt
        private void CollectionChangedMethodBitsFrei(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (OperandBitsFrei.Count == 1 && OperandBitsFrei[0].InformationsTyp == InformationsEnum.bitfolge) Eingabe = OperandBitsFrei[0].InformationsInhaltToString;
            CanExecute();
        }

        //Setzen der aktuellen Rolle und ändern des Icons, die sich die View abholt
        protected void setzeAktRolleView()
        {
            RolleEnum aktrolle = uebungsszenario.AktuelleRolle.RolleTyp;
            if (aktrolle == RolleEnum.Alice)
            {
                AktuelleRolleAnzeige = "Alice";
                RolleIcon = "/quaKrypto;component/Icons/Spiel/Alice/Alice_128px.png";
            }
            else if (aktrolle == RolleEnum.Bob)
            {
                AktuelleRolleAnzeige = "Bob";
                RolleIcon = "/quaKrypto;component/Icons/Spiel/Bob/Bob_128px.png";
            }
            else if (aktrolle == RolleEnum.Eve)
            {
                AktuelleRolleAnzeige = "Eve";
                RolleIcon = "/quaKrypto;component/Icons/Spiel/Eve/Eve_128px.png";
            }
            
        }

        //Setzt die aktuelle Phase, die sich die View zum Anzeigen abholt
        protected void setzeAktPhaseView()
        {
            uint aktphase = uebungsszenario.Variante.AktuellePhase;

            AktuellePhaseAnzeige = "Phase: " + aktphase.ToString();
            
        }

        //Strings leeren die die View in TextBoxen bindet 
        protected void ClearViewTextBox()
        {
            passwortFeld = "";
            Informationsname = "";
            Eingabe = "";
        }

        #region OperationenAusführen
        //Löst für alle Operationen Commands eine Prüfung der Startbedingung aus
        private void CanExecute()
        {
            this.BitFolgeErzeugen.RaiseCanExecuteChanged();
            this.EntVerschlüsseln.RaiseCanExecuteChanged();
            this.PhotonenErzeugen.RaiseCanExecuteChanged();
            this.PolschaErzeugen.RaiseCanExecuteChanged();
            this.Streichen.RaiseCanExecuteChanged();
            this.Vergleichen.RaiseCanExecuteChanged();
            this.ZahlErzeugen.RaiseCanExecuteChanged();
            this.BitMaskeGenerieren.RaiseCanExecuteChanged();
            this.BitfolgeNegieren.RaiseCanExecuteChanged();
            this.PhotonenZuBitfolge.RaiseCanExecuteChanged();
            this.TextGenerieren.RaiseCanExecuteChanged();
            this.TextLaengeBestimmen.RaiseCanExecuteChanged();
            this.BitsFreiBearbeiten.RaiseCanExecuteChanged();
            this.InformationUmbenennen.RaiseCanExecuteChanged();
        }

        //Erzeugt den Handlungsschritt bitfolge erzeugen
        private Information bitfolgeErzeugen()
        {
            //Prüfen ob der Benutzer eine manuelle Eingabe tätigen möchte
            if (EingabeBool)
            {
                //Benutzer will eine manuelle Eingabe tätigen, die Eingabe wird in eine Information umgewandelt
                Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);

                //Handlungsschritt ausführen
                return uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.bitfolgeGenerierenAngabe,
                                    angabe,
                                    Operand1[0], //Im Model Operand2 
                                    Informationsname,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            else
                //Benutzer will Bitfolge automatisch generieren lassen
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.bitfolgeGenerierenZahl,
                    Operand1[0],
                    null,
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }

        //Prüft ob die Startbedingung zum Erzeugen einer Bitfolge erfüllt ist
        private bool bitfolgeErzeugenStartBedingung()
        {
            //Prüfen, ob der Benutzer die Bitfolge manuell angeben will
            if (EingabeBool)
            {
                //Manuelle Eingabe aktiviert
                if (Informationsname == null ||
                    Eingabe == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl ||
                    Eingabe == "" ||
                    !StringToBitArray(Eingabe)) return false;//Prüfen ob die Eingabe in ein BitArray umgewandelt werden konnte
                return true;
            }
            else
            {
                //Automatische Eingabe aktiviert
                if (Informationsname == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl) return false;
                return true;
            }
        }

        //Ver- oder Entschlüsselt eine Information
        private Information entVerschlüsseln()
        {
            //Prüft ob es sich um einen nicht verschlüsselten Text handelt
            if (Operand1[0].InformationsTyp == InformationsEnum.asciiText)
            {
                //Operand1 ist Klartext
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.textVerschluesseln,
                    Operand1[0], //Text zum Verschlüsseln
                    Operand2[0], //Schlüssel zum Verschlüsseln
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
            }
            //Operand1 ist ein Verschlüsselter Text
            return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.textEntschluesseln,
                    Operand1[0], //Verschlüsselter Text
                    Operand2[0], //Schlüssel zum Entschlüsseln
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }

        //Prüft ob die Operation Ver- oder Entschlüsseln ausgeführt werden kann
        private bool entVerschlüsselnStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                    Operand1.Count != 1 ||
                    (Operand1[0].InformationsTyp != InformationsEnum.asciiText && Operand1[0].InformationsTyp != InformationsEnum.verschluesselterText) ||
                    Operand2.Count != 1 ||
                    Operand2[0].InformationsTyp != InformationsEnum.bitfolge) return false;
            //Länge des Texts bestimmen
            Information laenge = hilfsoperationen.TextLaengeBestimmen(-1, Operand1[0], null, "", null);
            //Prüfen ob der Schlüssel lang genug ist um den Text zu Ver oder Entschlüsseln
            if ((int)laenge.InformationsInhalt > ((bool[])Operand2[0].InformationsInhalt).Length) return false;
            return true;
        }

        //Erzeugen von Photonen
        private Information photonenErzeugen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.photonenGenerieren,
                Operand1[0], //Polarisationsschemata
                Operand2[0], //Schlüssel
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüft ob Photonen erzeugt werden können
        private bool photonenErzeugenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                    Operand1.Count != 1 ||
                    (Operand1[0].InformationsTyp != InformationsEnum.polarisationsschemata) ||
                    Operand2.Count != 1 ||
                    Operand2[0].InformationsTyp != InformationsEnum.bitfolge) return false;
            //Inhalte der Informationen erhalten
            bool[] op1Inhalt = (bool[])Operand1[0].InformationsInhalt;
            bool[] op2Inhalt = (bool[])Operand2[0].InformationsInhalt;
            //Prüfen ob die Polschata und der Schlüssel unterschiedlich lang sind, falls ja darf Operation nicht ausgeführt werden 
            if(op1Inhalt.Length != op2Inhalt.Length)return false;
            return true;
        }

        //Erzeugt ein Polarisationsschemata
        private Information polschaErzeugen()
        {
            //Prüfen ob der Benutzer eine manuelle Eingabe tätigen möchte
            if (EingabeBool)
            {
                //Benutzer will eine manuelle Eingabe tätigen, die Eingabe wird in eine Information umgewandelt
                Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);
                return uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.polarisationsschemataGenerierenAngabe,
                                    angabe,
                                    Operand1[0], //Im Model Operand2, Länge der zu erzeugenden Polschata
                                    Informationsname,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            else
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.polarisationsschemataGenerierenZahl,
                    Operand1[0], //Länge der zu erzeugenden Polschata
                    null,
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }
        private bool polschaErzeugenStartBedingung()
        {
            //Prüfen ob der Benutzer eine manuelle Eingabe tätigen möchte
            if (EingabeBool)
            {
                //Manuelle Eingabe aktiviert
                if (Informationsname == null ||
                    Eingabe == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl ||
                    Eingabe == "" ||
                    !StringToBitArrayPolscha(Eingabe)) return false; //Prüfen ob die Eingabe in eine Bitfolge konvertiert werden konnte
                return true;
            }
            else
            {
                //Automatische Eingabe aktiviert
                if (Informationsname == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl) return false;
                return true;
            }
        }

        //Streichen von Bits aus einer Bitfolge/Polschata
        private Information streichen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitsStreichen,
                Operand1[0], //Bitfolge dessen Bits gestrichen werden sollen
                Operand2[0], //Bitfolge, die angibt welche Bits gestrichen werden sollen
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool streichenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                    Operand1.Count != 1 ||
                    (Operand1[0].InformationsTyp != InformationsEnum.bitfolge) ||
                    Operand2.Count != 1 ||
                    Operand2[0].InformationsTyp != InformationsEnum.bitfolge) return false;
            //Inhalte der Informationen erhalten
            bool[] op1Inhalt = (bool[])Operand1[0].InformationsInhalt;
            bool[] op2Inhalt = (bool[])Operand2[0].InformationsInhalt;
            //Prüfen ob die Bitfolgen unterschiedlich lang sind, falls ja darf Operation nicht ausgeführt werden 
            if (op1Inhalt.Length != op2Inhalt.Length) return false;
            return true;
        }

        //Vergleicht zwei Bitfolgen/Polschata
        private Information vergleichen()
        {
            //Prüfen ob es sich um zwei Polschata handelt
            if (Operand1[0].InformationsTyp == InformationsEnum.polarisationsschemata)
            {
                //Polschata vergleichen
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.polschataVergleichen,
                    Operand1[0], //Polschata 1
                    Operand2[0], //Polschata 2
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
            }
            else
                //Bitfolgen vergleichen
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.bitfolgenVergleichen,
                    Operand1[0], //Bitfolge 1
                    Operand2[0], //Bitfolge 2
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }

        //Prüft ob die zwei Operanden verglichen werden können
        private bool vergleichenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.polarisationsschemata && Operand1[0].InformationsTyp != InformationsEnum.bitfolge) ||
                Operand2.Count != 1 ||
                Operand1[0].InformationsTyp != Operand2[0].InformationsTyp) return false;
            //Inhalte der Informationen erhalten
            bool[] op1Inhalt = (bool[])Operand1[0].InformationsInhalt;
            bool[] op2Inhalt = (bool[])Operand2[0].InformationsInhalt;
            //Prüfen ob die Bitfolgen unterschiedlich lang sind, falls ja darf Operation nicht ausgeführt werden 
            if (op1Inhalt.Length != op2Inhalt.Length) return false;
            return true;
        }

        //Erzeugt eine Zahl
        private Information zahlErzeugen()
        {
            //Die Eingabe wird in eine Information umgewandelt
            Information angabe = new Information(-1, "ManuelleEingabeZahl", InformationsEnum.zahl, convertedZahl, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.zahlGenerieren,
                                null,
                                angabe, //Im Model Operand2 die erzeugte Zahl
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }

        //Prüft, ob eine Zahl erzeugt werden kann
        private bool zahlErzeugenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Eingabe == null ||
                Eingabe == "" ||
                !StringToZahl(Eingabe)) return false;//Prüfen ob die Eingabe in eine Zahl umgewandelt werden konnte
            return true;
        }

        //Erzeugen einer Bitfolge mit einer bestimmten Anzahl an 1er => Bitmaske
        private Information bitMaskeGenerieren()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitmaskeGenerieren,
                Operand1[0], // Zahl Länge der Bitmaske
                Operand2[0], // Zahl Anzahl der 1er
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüft ob eine BitMaske erzeugt werden kann
        private bool bitMaskeGenerierenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.zahl) ||
                Operand2.Count != 1 ||
                Operand2[0].InformationsTyp != InformationsEnum.zahl) return false;
            return true;
        }

        //Dreht alle Bits eine Bitmaske um 1 => 0 und 0 => 1
        private Information bitfolgeNegieren()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitfolgeNegieren,
                Operand1[0], //Bitfolge die Negiert werden soll
                null,
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüft ob eine Bitfolge negiert werden kann
        private bool bitfolgeNegierenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.bitfolge)) return false;
            return true;
        }

        //Erzeugt aus einer Photonenfolge und Polschatafolge eine Bitfolge
        private Information photonenZuBitfolge()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.photonenZuBitfolge,
                Operand2[0], //Polschata
                Operand1[0], //Muss verkehrt herum angegeben werden, unscharfe Photonen
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüft, ob Photonen zu einer Bitfolge umgeformt werden können
        private bool photonenZuBitfolgeStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.unscharfePhotonen) ||
                Operand2.Count != 1 ||
                Operand2[0].InformationsTyp != InformationsEnum.polarisationsschemata) return false;
            //Inhalte der Informationen erhalten
            byte[] op1Inhalt = (byte[])Operand1[0].InformationsInhalt;
            bool[] op2Inhalt = (bool[])Operand2[0].InformationsInhalt;
            //Prüfen ob die Photonenfolge und die Polschata unterschiedlich lang sind, falls ja darf Operation nicht ausgeführt werden 
            if (op1Inhalt.Length != op2Inhalt.Length) return false;
            return true;
        }

        //Erzeugen eines Textes 
        private Information textGenerieren()
        {
            //Eingabe des Benutzers in eine Information umwandeln
            Information angabe = new Information(-1, "ManuelleEingabeZahl", InformationsEnum.asciiText, Eingabe, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.textGenerieren,
                null,
                angabe, //Im Model Operand2, Eingabe des Benutzers  
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüfen ob ein Text generiert werden kann
        private bool textGenerierenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Eingabe == null ||
                Eingabe == "" ||
                Eingabe.Length > 256) return false;
            return true;
        }

        //Bestimmt die Länge eines Textes/Bitfolge in Bits
        private Information textLaengeBestimmen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.textLaengeBestimmen,
                Operand1[0], //Text dessen länge in Bits berechntet werden soll
                null,
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }

        //Prüft ob die Länge eines Textes/Bitfolge bestimmt werden kann
        private bool textLaengeBestimmenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.asciiText &&
                 Operand1[0].InformationsTyp != InformationsEnum.bitfolge &&
                 Operand1[0].InformationsTyp != InformationsEnum.verschluesselterText)) return false;
            return true;
        }

        //Speichert eine Bitfolge ab die vom Benutzer eingegeben wurde
        private Information bitsFreiBearbeiten()
        {
            //Eingabe des Benutzers in eine Information umwandeln
            Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.bitsFreiBearbeiten,
                                angabe,
                                null,
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }

        //Prüft ob die Eingabe als Bitfolge abgespeichert werden kann
        private bool bitsFreiBearbeitenStartBedingung()
        {
            if (Informationsname == null ||
                Eingabe == null ||
                Informationsname == "" ||
                OperandBitsFrei.Count != 1 ||
                OperandBitsFrei[0].InformationsTyp != InformationsEnum.bitfolge ||
                Eingabe == "" ||
                !StringToBitArray(Eingabe)) return false;
            return true;
        }

        //Erzeugt eine neue Information, die den gleichen Inhalt hat, aber anders benannt ist
        private Information informationUmbenennen()
        {
            //Eingabe des Benutzers in eine Information umwandeln
            Information angabe = new Information(-1, "ManuelleEingabeText", InformationsEnum.asciiText, Informationsname, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.informationUmbenennen,
                                Operand1[0], //Information die Umbennant werden soll
                                angabe, //Eingabe des Benutzers
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }

        //Prüft ob eine Information umbenannt werden kann
        private bool informationUmbenennenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1) return false;
            return true;
        }
        #endregion

        #region Datentyp Konverter
        //Beinhaltet eine Bitfolge, die konvertiert werden konnte
        private bool[] convertedBitArray;

        //Wandelt eine Eingabe des Benutzers in ein BitArray und gibt zurück ob dies erfolgreich war
        private bool StringToBitArray(string eingabe)
        {
            //Eingabe muss existieren
            if (eingabe == null) return false;

            //Erzeugen einer Bool Arrays mit der Länge der Eingabe
            convertedBitArray = new bool[(eingabe.Length)];
            for (int i = 0; i < eingabe.Length; i++)
            {
                if (eingabe[i] == '0') convertedBitArray[i] = false;
                else if (eingabe[i] == '1') convertedBitArray[i] = true;
                else return false; //Falls ein anderes zeichen in der Eingabe steht schlägt die Operation fehl
            }
            return true;
        }

        //Wandelt eine Eingabe des Benutzers in ein BitArray und gibt zurück ob dies erfolgreich war (x und + als Eingabe zulässig)
        private bool StringToBitArrayPolscha(string eingabe)
        {
            //Eingabe muss existieren
            if (eingabe == null) return false;

            //Erzeugen einer Bool Arrays mit der Länge der Eingabe
            convertedBitArray = new bool[(eingabe.Length)];
            for (int i = 0; i < eingabe.Length; i++)
            {
                if (eingabe[i] == 'x' || eingabe[i] == 'X') convertedBitArray[i] = false;
                else if (eingabe[i] == '+') convertedBitArray[i] = true;
                else return false; //Falls ein anderes Zeichen in der Eingabe steht schlägt die Operation fehl
            }
            return true;
        }

        //Beinhaltet eine Zahl, die konvertiert werden konnte
        private int convertedZahl;

        //Wandelt eine Eingabe des Benutzers in eine Zahl um und gibt zurück ob dies erfolgreich war
        private bool StringToZahl(string eingabe)
        {
            //Eingabe muss existieren
            if (eingabe == null) return false;

            //Versuchen die Eingabe umzuwandeln
            if (Int32.TryParse(eingabe, out int j))
            {
                //Prüfen ob die Zahl zwischen 1 und 8191 liegt, falls ja Konvertierung erfolgreich
                if ((j > 0) && (j <= 8191))
                {
                    convertedZahl = j;
                    return true;
                }
            }
            return false;
        }
        #endregion

        //Ändert den Zustand der View
        protected void AenderZustand(Enums.SpielEnum spiel)
        {
            //Prüfen ob Spiel sich im Wartescreen befindet
            if (spiel == Enums.SpielEnum.warten)
            {
                //Spiel befindet sich im Wartescreen nur den Warte screen aktivieren und Rest deaktiveren
                WarteVisibility = Visibility.Visible;
                PasswortEingabeVisibility = Visibility.Hidden;
                SpielVisibility = Visibility.Hidden;
            }
            else if (spiel == Enums.SpielEnum.passwortEingabe)
            {
                //Spiel befindet sich in der Passworteingabe nur den Passworteingabe aktivieren und Rest deaktiveren
                WarteVisibility = Visibility.Hidden;
                PasswortEingabeVisibility = Visibility.Visible;
                SpielVisibility = Visibility.Hidden;
            }
            else if (spiel == Enums.SpielEnum.aktiv)
            {
                //Spiel befindet sich in der Passworteingabe nur den Passworteingabe aktivieren und Rest deaktiveren
                WarteVisibility = Visibility.Hidden;
                PasswortEingabeVisibility = Visibility.Hidden;
                SpielVisibility = Visibility.Visible;
            }
        }

        //Wird ausgelöst wenn sich die Phase ändert oder das Spiel startet
        protected void AktualisiereOperationenVisibility()
        {
            //Selektierte Operation auf 0 setzen
            CraftingFeldSelectedOperation = 0;

            //View auffordern neu die Visibility auszulesen
            EigenschaftWurdeGeändert(nameof(CraftingFeldSelectedOperation));
            EigenschaftWurdeGeändert(nameof(BitfolgeGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(PolarisationsschemataGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(PhotonenGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(BitMaskeGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(VergleichenVisibility));
            EigenschaftWurdeGeändert(nameof(BitfolgeNegierenVisibility));
            EigenschaftWurdeGeändert(nameof(PhotonenZuBitfolgeVisibility));
            EigenschaftWurdeGeändert(nameof(TextGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(TextLaengeBestimmenVisibility));
            EigenschaftWurdeGeändert(nameof(TextVerEntschlüsselnVisibility));
            EigenschaftWurdeGeändert(nameof(BitsStreichenVisibility));
            EigenschaftWurdeGeändert(nameof(BitsFreiBearbeitenVisibility));
            EigenschaftWurdeGeändert(nameof(ZahlGenerierenVisibility));
            EigenschaftWurdeGeändert(nameof(InformationUmbenennenVisibility));
        }
    }
}
