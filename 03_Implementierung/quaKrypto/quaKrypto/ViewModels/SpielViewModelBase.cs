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
using Xceed.Wpf.Toolkit;

namespace quaKrypto.ViewModels
{
    public class SpielViewModelBase : BaseViewModel
    {
        protected IUebungsszenario uebungsszenario;

        public Information CraftingFeldPhotonen { get; set; }
        public Information CraftingFeldPolarisation { get; set; }
        public Information CraftingFeldErgebnis { get; set; }
        public ObservableCollection<Information> Muelleimer { get; set; }
        public ObservableCollection<Information> Informationsablage { get; set; }

        protected List<Rolle> eigeneRollen;
        private Visibility warteVisibility;
        public Visibility WarteVisibility
        {
            get { return warteVisibility; }
            set { warteVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        private Visibility passwortEingabeVisibility;
        public Visibility PasswortEingabeVisibility
        {
            get { return passwortEingabeVisibility; }
            set { passwortEingabeVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        private Visibility spielVisibility;
        public Visibility SpielVisibility
        {
            get { return spielVisibility; }
            set { spielVisibility = value; this.EigenschaftWurdeGeändert(); }
        }

        private string passwort = string.Empty;

        public string passwortFeld
        {
            get { return passwort; }
            set { passwort = value; this.EigenschaftWurdeGeändert(); this.PasswortEingabe.RaiseCanExecuteChanged(); }
        }
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

        private string aktuellePhaseAnzeige;
        public string AktuellePhaseAnzeige
        {
            set
            {
                if(aktuellePhaseAnzeige != value)
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
        #region CraftingFeld

        private string informationsname;
        public string Informationsname
        {
            get { return informationsname; }
            set { informationsname = value; this.EigenschaftWurdeGeändert(); CanExecute(); }
        }

        private ObservableCollection<Information> operand1;
        public ObservableCollection<Information> Operand1
        {
            get { return operand1; }
            set
            {
                operand1 = value; this.EigenschaftWurdeGeändert();
            }
        }

        private ObservableCollection<Information> operandBitsFrei;
        public ObservableCollection<Information> OperandBitsFrei
        {
            get { return operandBitsFrei; }
            set
            {
                operandBitsFrei = value; this.EigenschaftWurdeGeändert();
            }
        }

        private ObservableCollection<Information> operand2;
        public ObservableCollection<Information> Operand2
        {
            get { return operand2; }
            set
            {
                operand2 = value; this.EigenschaftWurdeGeändert();
            }
        }

        private bool eingabeBool;
        public bool EingabeBool
        {
            get { return eingabeBool; }
            set
            {
                eingabeBool = value; this.EigenschaftWurdeGeändert(); CanExecute();
            }
        }

        private string eingabe;
        public string Eingabe
        {
            get { return eingabe; }
            set
            {
                eingabe = value; this.EigenschaftWurdeGeändert(); CanExecute();
            }
        }

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

        #region Commands

        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand PasswortEingabe { get; set; }

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
            /**
             * 
             * 
             * Zu Test Zwecken
             
             */
            /*
            Informationsablage = new ObservableCollection<Information>();
            BitArray arr = new BitArray(160, false);
            arr[1] = true;
            arr[2] = true;
            arr[2] = true;
            arr[20] = true;
            arr[30] = true;
            arr[31] = true;
            arr[32] = true;
            arr[20] = true;
            arr[50] = true;
            arr[51] = true;
            arr[62] = true;
            int length = 0;
            Information information = new Information(1, "Angabe", InformationsEnum.bitfolge, arr, null);
            Informationsablage.Add(information);
            //Arrange
            string text = "Hello";

            Information ergInformation = new Information(2, "Bitfolge", InformationsEnum.asciiText, text, null);
            Informationsablage.Add(ergInformation);
            /**
             * 
             * 
             * Zu Test Zwecken
             */

            this.uebungsszenario = uebungsszenario;
            this.eigeneRollen = eigeneRollen;
            if (this.eigeneRollen.Contains(this.uebungsszenario.AktuelleRolle)) AenderZustand(Enums.SpielEnum.passwortEingabe);
            else AenderZustand(Enums.SpielEnum.warten);

            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);



            BitFolgeErzeugen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(bitfolgeErzeugen());
                setzeAktPhaseView();
            }, (o) => bitfolgeErzeugenStartBedingung());

            EntVerschlüsseln = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(entVerschlüsseln());
                setzeAktPhaseView();
            }, (o) => entVerschlüsselnStartBedingung());

            PhotonenErzeugen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(photonenErzeugen());
                setzeAktPhaseView();
            }, (o) => photonenErzeugenStartBedingung());

            PolschaErzeugen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(polschaErzeugen());
                setzeAktPhaseView();
            }, (o) => polschaErzeugenStartBedingung());

            Streichen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(streichen());
                setzeAktPhaseView();
            }, (o) => streichenStartBedingung());

            Vergleichen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(vergleichen());
                setzeAktPhaseView();
            }, (o) => vergleichenStartBedingung());

            ZahlErzeugen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(zahlErzeugen());
                setzeAktPhaseView();
            }, (o) => zahlErzeugenStartBedingung());

            BitMaskeGenerieren = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(bitMaskeGenerieren());
                setzeAktPhaseView();
            }, (o) => bitMaskeGenerierenStartBedingung());

            BitfolgeNegieren = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(bitfolgeNegieren());
                setzeAktPhaseView();
            }, (o) => bitfolgeNegierenStartBedingung());

            PhotonenZuBitfolge = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(photonenZuBitfolge());
                setzeAktPhaseView();
            }, (o) => photonenZuBitfolgeStartBedingung());

            TextGenerieren = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(textGenerieren());
                setzeAktPhaseView();
            }, (o) => textGenerierenStartBedingung());

            TextLaengeBestimmen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(textLaengeBestimmen());
                setzeAktPhaseView();
            }, (o) => textLaengeBestimmenStartBedingung());

            BitsFreiBearbeiten = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(bitsFreiBearbeiten());
                setzeAktPhaseView();
            }, (o) => bitsFreiBearbeitenStartBedingung());

            InformationUmbenennen = new((o) =>
            {
                Ergebnis.Clear();
                Ergebnis.Add(informationUmbenennen());
                setzeAktPhaseView();
            }, (o) => informationUmbenennenStartBedingung());

            Operand1 = new ObservableCollection<Information>();
            Operand2 = new ObservableCollection<Information>();
            OperandBitsFrei = new ObservableCollection<Information>();
            Ergebnis = new ObservableCollection<Information>();
            
            Muelleimer = new ObservableCollection<Information>();

            Operand1.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            Operand2.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethod);
            OperandBitsFrei.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethodBitsFrei);

            Informationsablage = new ObservableCollection<Information>();
        }

        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanExecute();
        }

        private void CollectionChangedMethodBitsFrei(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (OperandBitsFrei.Count == 1 && OperandBitsFrei[0].InformationsTyp == InformationsEnum.bitfolge) Eingabe = OperandBitsFrei[0].InformationsInhaltToString;
            CanExecute();
        }

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

        protected void setzeAktPhaseView()
        {
            uint aktphase = uebungsszenario.Variante.AktuellePhase;

            AktuellePhaseAnzeige = "Phase: " + aktphase.ToString();    
        }

        #region OperationenAusführen
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

        private Information bitfolgeErzeugen()
        {
            if (EingabeBool)
            {
                Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);
                return uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.bitfolgeGenerierenAngabe,
                                    angabe,
                                    Operand1[0], //Im Model Operand2 
                                    Informationsname,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            else
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.bitfolgeGenerierenZahl,
                    Operand1[0],
                    null,
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }
        private bool bitfolgeErzeugenStartBedingung()
        {
            if (EingabeBool)
            {
                //Manuelle Eingabe aktiviert
                if (Informationsname == null ||
                    Eingabe == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl ||
                    Eingabe == "" ||
                    !StringToBitArray(Eingabe)) return false;
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

        private Information entVerschlüsseln()
        {
            if (Operand1[0].InformationsTyp == InformationsEnum.asciiText)
            {
                //Operand1 ist Klartext
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.textVerschluesseln,
                    Operand1[0],
                    Operand2[0],
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
            }
            //Operand1 ist ein Verschlüsselter Text
            return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.textEntschluesseln,
                    Operand1[0],
                    Operand2[0],
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }
        private bool entVerschlüsselnStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                    Operand1.Count != 1 ||
                    (Operand1[0].InformationsTyp != InformationsEnum.asciiText && Operand1[0].InformationsTyp != InformationsEnum.verschluesselterText) ||
                    Operand2.Count != 1 ||
                    Operand2[0].InformationsTyp != InformationsEnum.bitfolge) return false;
            return true;
        }

        private Information photonenErzeugen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.photonenGenerieren,
                Operand1[0],
                Operand2[0],
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool photonenErzeugenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                    Operand1.Count != 1 ||
                    (Operand1[0].InformationsTyp != InformationsEnum.polarisationsschemata) ||
                    Operand2.Count != 1 ||
                    Operand2[0].InformationsTyp != InformationsEnum.bitfolge) return false;
            return true;
        }
        private Information polschaErzeugen()
        {
            if (EingabeBool)
            {
                Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);
                return uebungsszenario.HandlungsschrittAusführenLassen(
                                    OperationsEnum.polarisationsschemataGenerierenAngabe,
                                    angabe,
                                    Operand1[0], //Im Model Operand2 
                                    Informationsname,
                                    uebungsszenario.AktuelleRolle.RolleTyp
                                    );
            }
            else
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.polarisationsschemataGenerierenZahl,
                    Operand1[0],
                    null,
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }
        private bool polschaErzeugenStartBedingung()
        {
            if (EingabeBool)
            {
                //Manuelle Eingabe aktiviert
                if (Informationsname == null ||
                    Eingabe == null ||
                    Informationsname == "" ||
                    Operand1.Count != 1 ||
                    Operand1[0].InformationsTyp != InformationsEnum.zahl ||
                    Eingabe == "" ||
                    !StringToBitArray(Eingabe)) return false;
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
        private Information streichen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitsStreichen,
                Operand1[0],
                Operand2[0],
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
            return true;
        }
        private Information vergleichen()
        {
            if (Operand1[0].InformationsTyp == InformationsEnum.polarisationsschemata)
            {
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.polschataVergleichen,
                    Operand1[0],
                    Operand2[0],
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
            }
            else
                return uebungsszenario.HandlungsschrittAusführenLassen(
                    OperationsEnum.bitfolgenVergleichen,
                    Operand1[0],
                    Operand2[0],
                    Informationsname,
                    uebungsszenario.AktuelleRolle.RolleTyp
                    );
        }
        private bool vergleichenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.polarisationsschemata && Operand1[0].InformationsTyp != InformationsEnum.bitfolge) ||
                Operand2.Count != 1 ||
                Operand1[0].InformationsTyp != Operand2[0].InformationsTyp) return false;
            return true;
        }
        private Information zahlErzeugen()
        {
            Information angabe = new Information(-1, "ManuelleEingabeZahl", InformationsEnum.zahl, convertedZahl, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.zahlGenerieren,
                                null,
                                angabe, //Im Model Operand2 
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }
        private bool zahlErzeugenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" || 
                Eingabe == null ||
                Eingabe == "" ||
                !StringToZahl(Eingabe)) return false;
            return true;
        }
        private Information bitMaskeGenerieren()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitmaskeGenerieren,
                Operand1[0],
                Operand2[0],
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
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

        private Information bitfolgeNegieren()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.bitfolgeNegieren,
                Operand1[0],
                null,
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool bitfolgeNegierenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.bitfolge)) return false;
            return true;
        }

        private Information photonenZuBitfolge()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.photonenZuBitfolge,
                Operand2[0],
                Operand1[0], //Muss verkehrt herum angegeben werden
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool photonenZuBitfolgeStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.unscharfePhotonen) ||
                Operand2.Count != 1 ||
                Operand2[0].InformationsTyp != InformationsEnum.polarisationsschemata) return false;
            return true;
        }

        private Information textGenerieren()
        {
            Information angabe = new Information(-1, "ManuelleEingabeZahl", InformationsEnum.zahl, Eingabe, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.textGenerieren,
                null,
                angabe, //Im Model Operand2 
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool textGenerierenStartBedingung()
        {
            if (Eingabe == null ||
                Eingabe == "") return false;
            return true;
        }

        private Information textLaengeBestimmen()
        {
            return uebungsszenario.HandlungsschrittAusführenLassen(
                OperationsEnum.textLaengeBestimmen,
                Operand1[0],
                null,
                Informationsname,
                uebungsszenario.AktuelleRolle.RolleTyp
                );
        }
        private bool textLaengeBestimmenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "" ||
                Operand1.Count != 1 ||
                (Operand1[0].InformationsTyp != InformationsEnum.asciiText &&
                 Operand1[0].InformationsTyp != InformationsEnum.bitfolge)) return false;
            return true;
        }

        private Information bitsFreiBearbeiten()
        {
            Information angabe = new Information(-1, "ManuelleEingabeBitFolge", InformationsEnum.bitfolge, convertedBitArray, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.bitsFreiBearbeiten,
                                angabe,
                                null,
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }
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

        private Information informationUmbenennen()
        {
            Information angabe = new Information(-1, "ManuelleEingabeText", InformationsEnum.asciiText, Informationsname, null);
            return uebungsszenario.HandlungsschrittAusführenLassen(
                                OperationsEnum.informationUmbenennen,
                                Operand1[0],
                                angabe,
                                Informationsname,
                                uebungsszenario.AktuelleRolle.RolleTyp
                                );
        }
        private bool informationUmbenennenStartBedingung()
        {
            if (Informationsname == null ||
                Informationsname == "") return false;
            return true;
        }
        #endregion

        #region Datentyp Konverter
        private bool[] convertedBitArray;
        private bool StringToBitArray(string eingabe)
        {
            if (eingabe == null) return false;
            convertedBitArray = new bool[(eingabe.Length)];
            for (int i = 0; i < eingabe.Length; i++)
            {
                if (eingabe[i] == '0') convertedBitArray[i] = false;
                else if (eingabe[i] == '1') convertedBitArray[i] = true;
                else return false;
            }
            return true;
        }

        private int convertedZahl;
        private bool StringToZahl(string eingabe)
        {
            if (eingabe == null) return false;

            if (Int32.TryParse(eingabe, out int j))
            {
                convertedZahl = j;
                return true;
            }
            return false;
        }


        #endregion
        protected void AenderZustand(Enums.SpielEnum spiel)
        {
            if (spiel == Enums.SpielEnum.warten)
            {
                WarteVisibility = Visibility.Visible;
                PasswortEingabeVisibility = Visibility.Hidden;
                SpielVisibility = Visibility.Hidden;
            }
            else if (spiel == Enums.SpielEnum.passwortEingabe)
            {
                WarteVisibility = Visibility.Hidden;
                PasswortEingabeVisibility = Visibility.Visible;
                SpielVisibility = Visibility.Hidden;
            }
            else if (spiel == Enums.SpielEnum.aktiv)
            {
                WarteVisibility = Visibility.Hidden;
                PasswortEingabeVisibility = Visibility.Hidden;
                SpielVisibility = Visibility.Visible;
            }
        }
    }
}
