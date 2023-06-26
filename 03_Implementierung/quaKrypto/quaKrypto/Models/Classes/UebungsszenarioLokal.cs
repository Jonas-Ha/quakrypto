// **********************************************************
// File: UebungsszenarioLokal.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Interfaces;
using quaKrypto.Models.Enums;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.VisualBasic;
using quaKrypto.Services;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioLokal : IUebungsszenario, INotifyPropertyChanged
    {
        private ObservableCollection<Rolle> rollen;
        private ReadOnlyObservableCollection<Rolle> rollenActual;
        private Rolle aktuelleRolle;
        private SchwierigkeitsgradEnum schwierigkeitsgrad;
        private IVariante variante;
        private uint startPhase;
        private uint endPhase;
        private Uebertragungskanal uebertragungskanal;
        private Aufzeichnung aufzeichnung;
        private string name;
        private bool beendet;
        public event PropertyChangedEventHandler? PropertyChanged;

        public UebungsszenarioLokal(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase, string name)
        {
            this.rollen = new ObservableCollection<Rolle>();
            this.rollenActual = new ReadOnlyObservableCollection<Rolle>(this.rollen);
            this.schwierigkeitsgrad = schwierigkeitsgrad;
            this.variante = variante;
            this.startPhase = startPhase;
            this.endPhase = endPhase;
            this.uebertragungskanal = new Uebertragungskanal();
            this.aufzeichnung = new Aufzeichnung();
            this.aufzeichnung.Handlungsschritte.CollectionChanged += this.variante.BerechneAktuellePhase;
            this.Variante.PropertyChanged += new PropertyChangedEventHandler(VarianteChanged);
            this.name = name;
            this.beendet = false;
            
            
        }

        public ReadOnlyObservableCollection<Rolle> Rollen => rollenActual;
        public Rolle AktuelleRolle { get { return aktuelleRolle; } }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get { return schwierigkeitsgrad; } }
        public IVariante Variante { get { return variante; } }
        public uint StartPhase { get { return startPhase; } }
        public uint EndPhase { get { return endPhase; } }
        public Uebertragungskanal Uebertragungskanal { get { return uebertragungskanal; } }
        public Aufzeichnung Aufzeichnung { get { return aufzeichnung; } }
        public string Name { get { return name; } }
        public bool Beendet { get { return beendet; } }
        public bool HostHatGestartet { get { return false; } }
        //Überprüft ob die Rolle bereits vergeben ist und falls nicht wird die Rolle hinzugefügt und gibt zurück ob die Rolle hinzugefügt
        public bool RolleHinzufuegen(Rolle rolle, bool eigeneRolle = false)
        {
            bool verfügbar = true;
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (rolle.RolleTyp == Rollen[i].RolleTyp) 
                {
                    verfügbar = false;
                    break;
                }
            }
            if(verfügbar)
            {
                rollen.Add(rolle);
            }
            return verfügbar;
        }

        public void GebeRolleFrei(RolleEnum rolle)
        {
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (rolle == Rollen[i].RolleTyp)
                {
                    rollen.RemoveAt(i);
                    break;
                }
            }
        }

        public bool Starten()
        {
            GeneriereInformationenFürRollen();

            var benoetigteRollen = Variante.MoeglicheRollen;
            if (Rollen.Count != benoetigteRollen.Count) return false;
            for(int i = 0; i < benoetigteRollen.Count; i++)
            {
                bool istvorhanden = false;
                for (int j = 0; j < Rollen.Count; j++)
                {
                    if (benoetigteRollen[i] == Rollen[j].RolleTyp)
                    {
                        istvorhanden = true;
                        break;
                    }
                }
                if (!istvorhanden) return false;
            }
            RolleEnum aktRolle = Variante.NaechsteRolle();
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (aktRolle == Rollen[i].RolleTyp)
                {
                    aktuelleRolle = Rollen[i];
                    break;
                }
            }
            return true;
        }

        public void NaechsterZug()
        {
            //List<Handlungsschritt> handlungsschritte = aktuelleRolle.handlungsschritte;

            //Aufzeichnung.HaengeListeHandlungsschritteAn(handlungsschritte.ToList());
            //Die Handlungsschritte müssen hier noch überprüft werden um die Informationsablage auf den richtigen Stand zu bekommen
            aktuelleRolle.handlungsschritte.Clear();

            RolleEnum aktRolle = Variante.NaechsteRolle();
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (aktRolle == Rollen[i].RolleTyp)
                {
                    int zaehlerstand = aktuelleRolle.InformationsZaehler;
                    aktuelleRolle = Rollen[i];
                    aktuelleRolle.AktualisiereInformationsZaehler(zaehlerstand);
                    break;
                }
            }
            PropertyHasChanged(nameof(Rolle));
        }

        public bool GebeBildschirmFrei(string Passwort)
        {
            return aktuelleRolle.BeginneZug(Passwort);
        }

        public Information HandlungsschrittAusführenLassen(Enums.OperationsEnum operationsTyp, Information operand1, object operand2, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
            Handlungsschritt handlungsschritt = aktuelleRolle.ErzeugeHandlungsschritt(operationsTyp, operand1, operand2, ergebnisInformationsName, rolle);
            handlungsschritt.AktuellePhase = Variante.AktuellePhase;
            if (operationsTyp == OperationsEnum.nachrichtSenden) Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
            Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);
            return handlungsschritt.Ergebnis;
        }

        //Speichert eine Information in der Ablage der aktuellen Rolle
        public void SpeichereInformationenAb(Information information)
        {
            aktuelleRolle.SpeicherInformationAb(information);
        }

        public void LoescheInformation(int informationID)
        {
            aktuelleRolle.LoescheInformation(informationID);
        }

        public void LoescheInformationAusUebertragungskanal(KanalEnum kanal, int informatonsID)
        {
            Uebertragungskanal.LoescheNachricht(kanal, informatonsID);
        }

        public void Beenden()
        {
            beendet = true;
            PropertyHasChanged(nameof(Beendet));
        }

        private void VarianteChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (variante.AktuellePhase >= endPhase)
            {
                Beenden();
                return;
            }
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }

        private void GeneriereInformationenFürRollen()
        {
            if (startPhase > 4 || startPhase < 1) return;

            Rolle rolleAlice = rollen.First(r => r.RolleTyp == RolleEnum.Alice), rolleBob = rollen.First(r => r.RolleTyp == RolleEnum.Bob), rolleEve = rollen.FirstOrDefault(r => r.RolleTyp == RolleEnum.Eve) ?? new Rolle(RolleEnum.Eve, "");

            Operationen operationen = new();

            int zähler = -1;

            Information ausgangsText = new(zähler--, "Geheimtext", InformationsEnum.asciiText, StandardTexte.BekommeZufälligenText());
            Information mindestSchlüssellänge = operationen.TextLaengeBestimmen(zähler--, ausgangsText, null, "Mindestschlüssellänge");
            Information schlüssellänge = new(zähler--, "Schlüssellänge", InformationsEnum.zahl, (int)mindestSchlüssellänge.InformationsInhalt * 3);

            switch (Variante)
            {
                case VarianteNormalerAblauf:

                    Information schlüsselbits1Alice = operationen.BitfolgeGenerierenZahl(zähler--, schlüssellänge, null, "Schlüsselbits - Anfang");
                    Information polschataAlice = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellänge, null, "Polarisationsschemata");
                    Information photonenAlice = operationen.PhotonenGenerieren(zähler--, polschataAlice, schlüsselbits1Alice, "Photonen");

                    Information polschataBob = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellänge, null, "Polarisationsschemata");
                    Information unscharfePhotonenBob = operationen.NachrichtSenden(zähler--, photonenAlice, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information schlüsselbits1Bob = operationen.PhotonenZuBitfolge(zähler--, polschataBob, unscharfePhotonenBob, "Schlüsselbits - Anfang");
                    //PHASE 1 ENDE
                    Information polschataDifferenzBob = operationen.PolschataVergleichen(zähler--, polschataAlice, polschataBob, "Unterschied Polarisationsschemata");
                    Information schlüsselbits2Bob = operationen.BitsStreichen(zähler--, schlüsselbits1Bob, polschataDifferenzBob, "Schlüsselbits - Gestrichen");

                    Information schlüsselbits2Alice = operationen.BitsStreichen(zähler--, schlüsselbits1Alice, polschataDifferenzBob, "Schlüsselbits - Gestrichen");
                    //PHASE 2 MENDE
                    Information prüfbitAnzahl = new(zähler--, "Anzahl der Prüfbits", InformationsEnum.zahl, ((bool[])schlüsselbits2Alice.InformationsInhalt).Length / 10);
                    Information längePrüfmaske = new(zähler--, "Länge Prüfmaske", InformationsEnum.zahl, ((bool[])schlüsselbits2Alice.InformationsInhalt).Length);
                    Information prüfmaske = operationen.BitmaskeGenerieren(zähler--, längePrüfmaske, prüfbitAnzahl, "Prüfmaske");
                    Information prüfbitsAlice = operationen.BitsStreichen(zähler--, schlüsselbits2Alice, operationen.BitfolgeNegieren(zähler--, prüfmaske, null, ""), "Prüfbits");
                    Information schlüsselbits3Alice = operationen.BitsStreichen(zähler--, schlüsselbits2Alice, prüfmaske, "Schlüsselbits - Final");

                    Information prüfbitsBob = operationen.BitsStreichen(zähler--, schlüsselbits2Bob, operationen.BitfolgeNegieren(zähler--, prüfmaske, null, ""), "Prüfbits");
                    Information schlüsselbits3Bob = operationen.BitsStreichen(zähler--, schlüsselbits2Bob, prüfmaske, "Schlüsselbits - Final");

                    Information prüfbitsDifferenzAlice = operationen.BitfolgenVergleichen(zähler--, prüfbitsAlice, prüfbitsBob, "Unterschied Prüfbits");
                    //PHASE 3 MENDE
                    if (startPhase >= 1)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsText, true);
                        rolleAlice.SpeicherInformationAb(mindestSchlüssellänge, true);
                        rolleAlice.SpeicherInformationAb(schlüssellänge, true);
                        rolleBob.SpeicherInformationAb(schlüssellänge, true);
                    }
                    if (startPhase >= 2)
                    {
                        rolleAlice.SpeicherInformationAb(schlüsselbits1Alice, true);
                        rolleAlice.SpeicherInformationAb(polschataAlice, true);
                        rolleAlice.SpeicherInformationAb(photonenAlice, true);
                        rolleBob.SpeicherInformationAb(polschataBob, true);
                        rolleBob.SpeicherInformationAb(unscharfePhotonenBob, true);
                        rolleBob.SpeicherInformationAb(schlüsselbits1Bob, true);
                    }
                    if (startPhase >= 3)
                    {
                        rolleBob.SpeicherInformationAb(polschataAlice, true);
                        rolleBob.SpeicherInformationAb(polschataDifferenzBob, true);
                        rolleBob.SpeicherInformationAb(schlüsselbits2Bob, true);
                        rolleAlice.SpeicherInformationAb(polschataDifferenzBob, true);
                        rolleAlice.SpeicherInformationAb(schlüsselbits2Alice, true);
                    }
                    if (startPhase >= 4)
                    {
                        rolleAlice.SpeicherInformationAb(prüfbitAnzahl, true);
                        rolleAlice.SpeicherInformationAb(längePrüfmaske, true);
                        rolleAlice.SpeicherInformationAb(prüfmaske, true);
                        rolleAlice.SpeicherInformationAb(prüfbitsAlice, true);
                        rolleAlice.SpeicherInformationAb(schlüsselbits3Alice, true);
                        rolleBob.SpeicherInformationAb(prüfmaske, true);
                        rolleBob.SpeicherInformationAb(prüfbitsBob, true);
                        rolleBob.SpeicherInformationAb(schlüsselbits3Bob, true);
                        rolleAlice.SpeicherInformationAb(prüfbitsBob, true);
                        rolleAlice.SpeicherInformationAb(prüfbitsDifferenzAlice, true);
                    }
                    break;
                case VarianteAbhoeren:
                    switch (startPhase)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                    break;
                case VarianteManInTheMiddle:
                    switch (startPhase)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                    break;
            }
        }
    }
}
