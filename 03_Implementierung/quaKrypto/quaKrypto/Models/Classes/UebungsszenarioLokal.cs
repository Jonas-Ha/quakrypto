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
            if (verfügbar)
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
            var benoetigteRollen = Variante.MoeglicheRollen;
            if (Rollen.Count != benoetigteRollen.Count) return false;
            for (int i = 0; i < benoetigteRollen.Count; i++)
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
            GeneriereInformationenFürRollen();
            RolleEnum aktRolle = Variante.NaechsteRolle();
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (aktRolle == Rollen[i].RolleTyp)
                {
                    aktuelleRolle = Rollen[i];
                    break;
                }
            }
            PropertyHasChanged(nameof(aktuelleRolle));
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
            PropertyHasChanged(nameof(aktuelleRolle));
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

        public int GeneriereInformationenFürRollen(int hostSeed = -1)
        {
            if (startPhase > 4 || startPhase < 1) return -1;

            Rolle rolleAlice = rollen.First(r => r.RolleTyp == RolleEnum.Alice), rolleBob = rollen.First(r => r.RolleTyp == RolleEnum.Bob), rolleEve = rollen.FirstOrDefault(r => r.RolleTyp == RolleEnum.Eve) ?? new Rolle(RolleEnum.Eve, "");
            int seed = hostSeed == -1 ? (int)DateTime.Now.Ticks : hostSeed;
            StandardTexte.Seed = seed;
            Operationen operationen = new(seed);

            int schwierigkeit = (int)Schwierigkeitsgrad;
            int zähler = -1;

            Information ausgangsTextAlice = new(zähler--, "Geheimtext", InformationsEnum.asciiText, StandardTexte.BekommeZufälligenText());
            Information mindestSchlüssellängeAlice = operationen.TextLaengeBestimmen(zähler--, ausgangsTextAlice, null, "Mindestschlüssellänge");
            Information schlüssellängeAlice = new(zähler--, "Schlüssellänge", InformationsEnum.zahl, (int)mindestSchlüssellängeAlice.InformationsInhalt * 3);

            switch (Variante)
            {
                case VarianteNormalerAblauf:
                    //Phase 1 Beginn
                    Information NAschlüsselbits1Alice = operationen.BitfolgeGenerierenZahl(zähler--, schlüssellängeAlice, null, "Schlüsselbits - Anfang");
                    Information NApolschataAlice = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information NAphotonenAlice = operationen.PhotonenGenerieren(zähler--, NApolschataAlice, NAschlüsselbits1Alice, "Photonen");

                    Information NApolschataBob = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information NAunscharfePhotonenBob = operationen.NachrichtSenden(zähler--, NAphotonenAlice, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information NAschlüsselbits1Bob = operationen.PhotonenZuBitfolge(zähler--, NApolschataBob, NAunscharfePhotonenBob, "Schlüsselbits - Anfang");
                    //Phase 2 Beginn
                    Information NApolschataDifferenzBob = operationen.PolschataVergleichen(zähler--, NApolschataAlice, NApolschataBob, "Unterschied Polarisationsschemata");
                    Information NAschlüsselbits2Bob = operationen.BitsStreichen(zähler--, NAschlüsselbits1Bob, NApolschataDifferenzBob, "Schlüsselbits - Gestrichen");

                    Information NAschlüsselbits2Alice = operationen.BitsStreichen(zähler--, NAschlüsselbits1Alice, NApolschataDifferenzBob, "Schlüsselbits - Gestrichen");
                    //Phase 3 Beginn
                    Information NAprüfbitAnzahl = new(zähler--, "Anzahl der Prüfbits", InformationsEnum.zahl, ((bool[])NAschlüsselbits2Alice.InformationsInhalt).Length / 10);
                    Information NAlängePrüfmaske = new(zähler--, "Länge Prüfmaske", InformationsEnum.zahl, ((bool[])NAschlüsselbits2Alice.InformationsInhalt).Length);
                    Information NAprüfmaske = operationen.BitmaskeGenerieren(zähler--, NAlängePrüfmaske, NAprüfbitAnzahl, "Prüfmaske");
                    Information NAprüfbitsAlice = operationen.BitsStreichen(zähler--, NAschlüsselbits2Alice, operationen.BitfolgeNegieren(zähler--, NAprüfmaske, null, ""), "Prüfbits");
                    Information NAschlüsselbits3Alice = operationen.BitsStreichen(zähler--, NAschlüsselbits2Alice, NAprüfmaske, "Schlüsselbits - Final");

                    Information NAprüfbitsBob = operationen.BitsStreichen(zähler--, NAschlüsselbits2Bob, operationen.BitfolgeNegieren(zähler--, NAprüfmaske, null, ""), "Prüfbits");
                    Information NAschlüsselbits3Bob = operationen.BitsStreichen(zähler--, NAschlüsselbits2Bob, NAprüfmaske, "Schlüsselbits - Final");

                    Information NAprüfbitsDifferenzAlice = operationen.BitfolgenVergleichen(zähler--, NAprüfbitsAlice, NAprüfbitsBob, "Unterschied Prüfbits");
                    //Phase 4 Beginn
                    // - -- - -- - - 
                    if (startPhase == 1)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                    }
                    else if (startPhase == 2)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //-----------//
                        rolleAlice.SpeicherInformationAb(NAschlüsselbits1Alice, true);
                        rolleAlice.SpeicherInformationAb(NApolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAphotonenAlice, true);
                        rolleBob.SpeicherInformationAb(NApolschataBob, true);
                        rolleBob.SpeicherInformationAb(NAschlüsselbits1Bob, true);
                    }
                    else if (startPhase == 3)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAschlüsselbits1Alice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NApolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAphotonenAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(NApolschataBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NAschlüsselbits1Bob, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(NApolschataAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NApolschataDifferenzBob, true);
                        rolleBob.SpeicherInformationAb(NAschlüsselbits2Bob, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NApolschataDifferenzBob, true);
                        rolleAlice.SpeicherInformationAb(NAschlüsselbits2Alice, true);
                    }
                    else if (startPhase == 4)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAschlüsselbits1Alice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NApolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAphotonenAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(NApolschataBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NAschlüsselbits1Bob, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(NApolschataAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NApolschataDifferenzBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NAschlüsselbits2Bob, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NApolschataDifferenzBob, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NAschlüsselbits2Alice, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAprüfbitAnzahl, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(NAlängePrüfmaske, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NAprüfmaske, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NAprüfbitsAlice, true);
                        rolleAlice.SpeicherInformationAb(NAschlüsselbits3Alice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(NAprüfmaske, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(NAprüfbitsBob, true);
                        rolleBob.SpeicherInformationAb(NAschlüsselbits3Bob, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(NAprüfbitsBob, true);
                        rolleAlice.SpeicherInformationAb(NAprüfbitsDifferenzAlice, true);
                    }
                    break;
                case VarianteAbhoeren:
                    //PHASE 1 Beginn
                    Information VAschlüsselbits1Alice = operationen.BitfolgeGenerierenZahl(zähler--, schlüssellängeAlice, null, "Schlüsselbits - Anfang");
                    Information VApolschataAlice = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information VAphotonenAlice = operationen.PhotonenGenerieren(zähler--, VApolschataAlice, VAschlüsselbits1Alice, "Photonen");

                    Information VApolschataEve = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information VAunscharfePhotonenEve = operationen.NachrichtSenden(zähler--, VAphotonenAlice, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information VAschlüsselbits1Eve = operationen.PhotonenZuBitfolge(zähler--, VApolschataEve, VAunscharfePhotonenEve, "Schlüsselbits - Anfang");
                    Information VAphotonenEve = operationen.PhotonenGenerieren(zähler--, VApolschataEve, VAschlüsselbits1Eve, "Photonen an Bob");

                    Information VApolschataBob = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information VAunscharfePhotonenBob = operationen.NachrichtSenden(zähler--, VAphotonenEve, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information VAschlüsselbits1Bob = operationen.PhotonenZuBitfolge(zähler--, VApolschataBob, VAunscharfePhotonenBob, "Schlüsselbits - Anfang");
                    //PHASE 2 Beginn
                    Information VApolschataDifferenzEve = operationen.PolschataVergleichen(zähler--, VApolschataAlice, VApolschataEve, "Unterschied Polarisationsschemata zu Alice");

                    Information VApolschataDifferenzBob = operationen.PolschataVergleichen(zähler--, VApolschataAlice, VApolschataBob, "Unterschied Polarisationsschemata");
                    Information VAschlüsselbits2Bob = operationen.BitsStreichen(zähler--, VAschlüsselbits1Bob, VApolschataDifferenzBob, "Schlüsselbits - Gestrichen");

                    Information VAschlüsselbits2Eve = operationen.BitsStreichen(zähler--, VAschlüsselbits1Eve, VApolschataDifferenzBob, "Schlüsselbits - Gestrichen mit Bob");
                    Information VAbitmaskeDifferenzZuBobEve = operationen.BitsStreichen(zähler--, VApolschataDifferenzEve, VApolschataDifferenzBob, "Unterschied zum Unterschied zwischen Alice und Bob");

                    Information VAschlüsselbits2Alice = operationen.BitsStreichen(zähler--, VAschlüsselbits1Alice, VApolschataDifferenzBob, "Schlüsselbits - Gestrichen");
                    //PHASE 3 Beginn
                    Information VAprüfbitAnzahl = new(zähler--, "Anzahl der Prüfbits", InformationsEnum.zahl, ((bool[])VAschlüsselbits2Alice.InformationsInhalt).Length / 10);
                    Information VAlängePrüfmaske = new(zähler--, "Länge Prüfmaske", InformationsEnum.zahl, ((bool[])VAschlüsselbits2Alice.InformationsInhalt).Length);
                    Information VAprüfmaske = operationen.BitmaskeGenerieren(zähler--, VAlängePrüfmaske, VAprüfbitAnzahl, "Prüfmaske");
                    Information VAprüfbitsAlice = operationen.BitsStreichen(zähler--, VAschlüsselbits2Alice, operationen.BitfolgeNegieren(zähler--, VAprüfmaske, null, ""), "Prüfbits");
                    Information VAschlüsselbits3Alice = operationen.BitsStreichen(zähler--, VAschlüsselbits2Alice, VAprüfmaske, "Schlüsselbits - Final");

                    Information VAprüfbitsEve = operationen.BitsStreichen(zähler--, VAschlüsselbits2Eve, operationen.BitfolgeNegieren(zähler--, VAprüfmaske, null, ""), "Prüfbits");
                    Information VAschlüsselbits3Eve = operationen.BitsStreichen(zähler--, VAschlüsselbits2Eve, VAprüfmaske, "Schlüsselbits - Final");

                    Information VAprüfbitsBob = operationen.BitsStreichen(zähler--, VAschlüsselbits2Bob, operationen.BitfolgeNegieren(zähler--, VAprüfmaske, null, ""), "Prüfbits");
                    Information VAschlüsselbits3Bob = operationen.BitsStreichen(zähler--, VAschlüsselbits2Bob, VAprüfmaske, "Schlüsselbits - Final");

                    Information VAprüfbitsDifferenzEve = operationen.BitfolgenVergleichen(zähler--, VAprüfbitsEve, VAprüfbitsBob, "Unterschied eigener Prüfbits zu denen von Bob");

                    Information VAprüfbitsDifferenzAlice = operationen.BitfolgenVergleichen(zähler--, VAprüfbitsAlice, VAprüfbitsBob, "Unterschied Prüfbits");
                    //PHASE 4 Beginn
                    // - -- - -- - -
                    if (startPhase == 1)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                    }
                    else if (startPhase == 2)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //---------//
                        rolleAlice.SpeicherInformationAb(VAschlüsselbits1Alice, true);
                        rolleAlice.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAphotonenAlice, true);
                        rolleEve.SpeicherInformationAb(VApolschataEve, true);
                        rolleEve.SpeicherInformationAb(VAschlüsselbits1Eve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VAphotonenEve, true);
                        rolleBob.SpeicherInformationAb(VApolschataBob, true);
                        rolleBob.SpeicherInformationAb(VAschlüsselbits1Bob, true);
                    }
                    else if (startPhase == 3)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //---------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAschlüsselbits1Alice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VAphotonenAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VApolschataEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VAschlüsselbits1Eve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VAphotonenEve, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(VApolschataBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VAschlüsselbits1Bob, true);
                        //---------//
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VApolschataDifferenzEve, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        rolleBob.SpeicherInformationAb(VAschlüsselbits2Bob, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        rolleEve.SpeicherInformationAb(VAschlüsselbits2Eve, true);
                        rolleEve.SpeicherInformationAb(VAbitmaskeDifferenzZuBobEve, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        rolleAlice.SpeicherInformationAb(VAschlüsselbits2Alice, true);
                    }
                    else if (startPhase == 4)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(schlüssellängeAlice, true);
                        //---------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAschlüsselbits1Alice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAphotonenAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VApolschataEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VAschlüsselbits1Eve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VAphotonenEve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VApolschataBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VAschlüsselbits1Bob, true);
                        //---------//
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VApolschataDifferenzEve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VApolschataAlice, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(VAschlüsselbits2Bob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VAschlüsselbits2Eve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VAbitmaskeDifferenzZuBobEve, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VApolschataDifferenzBob, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAschlüsselbits2Alice, true);
                        //---------//
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VAprüfbitAnzahl, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAlängePrüfmaske, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(VAprüfmaske, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VAprüfbitsAlice, true);
                        rolleAlice.SpeicherInformationAb(VAschlüsselbits3Alice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(VAprüfmaske, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VAprüfbitsEve, true);
                        rolleEve.SpeicherInformationAb(VAschlüsselbits3Eve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(VAprüfmaske, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(VAprüfbitsBob, true);
                        rolleBob.SpeicherInformationAb(VAschlüsselbits3Bob, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(VAprüfbitsBob, true);
                        rolleEve.SpeicherInformationAb(VAprüfbitsDifferenzEve, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(VAprüfbitsBob, true);
                        rolleAlice.SpeicherInformationAb(VAprüfbitsDifferenzAlice, true);
                    }
                    break;
                case VarianteManInTheMiddle:
                    //PHASE 0 Beginn
                    Information MITMausgangsTextEve = new(zähler--, "Geheimtext Eve - Bob", InformationsEnum.asciiText, StandardTexte.BekommeZufälligenText());
                    Information MITMmindestSchlüssellängeEve = operationen.TextLaengeBestimmen(zähler--, MITMausgangsTextEve, null, "Mindestschlüssellänge Eve - Bob");
                    Information MITMschlüssellängeEve = new(zähler--, "Schlüssellänge Eve - Bob", InformationsEnum.zahl, (int)MITMmindestSchlüssellängeEve.InformationsInhalt * 3);
                    //PHASE 1 Beginn
                    Information MITMschlüsselbits1Alice = operationen.BitfolgeGenerierenZahl(zähler--, schlüssellängeAlice, null, "Schlüsselbits - Anfang");
                    Information MITMpolschataAlice = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata");
                    Information MITMphotonenAlice = operationen.PhotonenGenerieren(zähler--, MITMpolschataAlice, MITMschlüsselbits1Alice, "Photonen");

                    Information MITMpolschataAliceEve = operationen.PolarisationsschemataGenerierenZahl(zähler--, schlüssellängeAlice, null, "Polarisationsschemata Alice - Eve");
                    Information MITMunscharfePhotonenEve = operationen.NachrichtSenden(zähler--, MITMphotonenAlice, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information MITMschlüsselbits1AliceEve = operationen.PhotonenZuBitfolge(zähler--, MITMpolschataAliceEve, MITMunscharfePhotonenEve, "Schlüsselbits Alice - Eve - Anfang");
                    Information MITMschlüsselbits1EveBob = operationen.BitfolgeGenerierenZahl(zähler--, MITMschlüssellängeEve, null, "Schlüsselbits Eve - Bob - Anfang");
                    Information MITMpolschataEveBob = operationen.PolarisationsschemataGenerierenZahl(zähler--, MITMschlüssellängeEve, null, "Polarisationsschemata Eve - Bob");
                    Information MITMphotonenEveBob = operationen.PhotonenGenerieren(zähler--, MITMpolschataEveBob, MITMschlüsselbits1EveBob, "Photonen An Bob");

                    Information MITMpolschataBob = operationen.PolarisationsschemataGenerierenZahl(zähler--, MITMschlüssellängeEve, null, "Polarisationsschemata");
                    Information MITMunscharfePhotonenBob = operationen.NachrichtSenden(zähler--, MITMphotonenEveBob, new Information(zähler--, "", InformationsEnum.keinInhalt, RolleEnum.Bob, RolleEnum.Bob, RolleEnum.Alice), "Unscharfe Photonen von Alice", RolleEnum.Alice);
                    Information MITMschlüsselbits1Bob = operationen.PhotonenZuBitfolge(zähler--, MITMpolschataBob, MITMunscharfePhotonenBob, "Schlüsselbits - Anfang");
                    //PHASE 2 Beginn
                    Information MITMpolschataDifferenzAliceEve = operationen.PolschataVergleichen(zähler--, MITMpolschataAlice, MITMpolschataAliceEve, "Unterschied Polarisationsschemata zu Alice");
                    Information MITMschlüsselbits2AliceEve = operationen.BitsStreichen(zähler--, MITMschlüsselbits1AliceEve, MITMpolschataDifferenzAliceEve, "Schlüsselbits Alice - Eve - Gestrichen");

                    Information MITMpolschataDifferenzBob = operationen.PolschataVergleichen(zähler--, MITMpolschataEveBob, MITMpolschataBob, "Unterschied Polarisationsschemata");
                    Information MITMschlüsselbits2Bob = operationen.BitsStreichen(zähler--, MITMschlüsselbits1Bob, MITMpolschataDifferenzBob, "Schlüsselbits - Gestrichen");

                    Information MITMschlüsselbits2EveBob = operationen.BitsStreichen(zähler--, MITMschlüsselbits1EveBob, MITMpolschataDifferenzBob, "Schlüsselbits Eve - Bob - Gestrichen");

                    Information MITMschlüsselbits2Alice = operationen.BitsStreichen(zähler--, MITMschlüsselbits1Alice, MITMpolschataDifferenzAliceEve, "Schlüsselbits - Gestrichen");
                    //PHASE 3 Beginn
                    Information MITMprüfbitAnzahlAlice = new(zähler--, "Anzahl der Prüfbits", InformationsEnum.zahl, ((bool[])MITMschlüsselbits2Alice.InformationsInhalt).Length / 10);
                    Information MITMlängePrüfmaskeAlice = new(zähler--, "Länge Prüfmaske", InformationsEnum.zahl, ((bool[])MITMschlüsselbits2Alice.InformationsInhalt).Length);
                    Information MITMprüfmaskeAlice = operationen.BitmaskeGenerieren(zähler--, MITMlängePrüfmaskeAlice, MITMprüfbitAnzahlAlice, "Prüfmaske");
                    Information MITMprüfbitsAlice = operationen.BitsStreichen(zähler--, MITMschlüsselbits2Alice, operationen.BitfolgeNegieren(zähler--, MITMprüfmaskeAlice, null, ""), "Prüfbits");
                    Information MITMschlüsselbits3Alice = operationen.BitsStreichen(zähler--, MITMschlüsselbits2Alice, MITMprüfmaskeAlice, "Schlüsselbits - Final");

                    Information MITMprüfbitsAliceEve = operationen.BitsStreichen(zähler--, MITMschlüsselbits2AliceEve, operationen.BitfolgeNegieren(zähler--, MITMprüfmaskeAlice, null, ""), "Prüfbits Alice - Eve");
                    Information MITMschlüsselbits3AliceEve = operationen.BitsStreichen(zähler--, MITMschlüsselbits2AliceEve, MITMprüfmaskeAlice, "Schlüsselbits Alice - Eve - Final");
                    Information MITMprüfbitAnzahlEveBob = new(zähler--, "Anzahl der Prüfbits mit Bob", InformationsEnum.zahl, ((bool[])MITMschlüsselbits2EveBob.InformationsInhalt).Length / 10);
                    Information MITMlängePrüfmaskeEveBob = new(zähler--, "Länge Prüfmaske mit Bob", InformationsEnum.zahl, ((bool[])MITMschlüsselbits2EveBob.InformationsInhalt).Length);
                    Information MITMprüfmaskeEveBob = operationen.BitmaskeGenerieren(zähler--, MITMlängePrüfmaskeEveBob, MITMprüfbitAnzahlEveBob, "Prüfmaske mit Bob");
                    Information MITMprüfbitsEveBob = operationen.BitsStreichen(zähler--, MITMschlüsselbits2EveBob, operationen.BitfolgeNegieren(zähler--, MITMprüfmaskeEveBob, null, ""), "Prüfbits mit Bob");
                    Information MITMschlüsselbits3EveBob = operationen.BitsStreichen(zähler--, MITMschlüsselbits2EveBob, MITMprüfmaskeEveBob, "Schlüsselbits Eve - Bob - Final");

                    Information MITMprüfbitsBob = operationen.BitsStreichen(zähler--, MITMschlüsselbits2Bob, operationen.BitfolgeNegieren(zähler--, MITMprüfmaskeEveBob, null, ""), "Prüfbits");
                    Information MITMschlüsselbits3Bob = operationen.BitsStreichen(zähler--, MITMschlüsselbits2Bob, MITMprüfmaskeEveBob, "Schlüsselbits - Final");

                    Information MITMprüfbitsDifferenzEve = operationen.BitfolgenVergleichen(zähler--, MITMprüfbitsEveBob, MITMprüfbitsBob, "Unterschied eigener Prüfbits zu denen von Bob");

                    Information MITMprüfbitsDifferenzAlice = operationen.BitfolgenVergleichen(zähler--, MITMprüfbitsAlice, MITMprüfbitsAliceEve, "Unterschied Prüfbits");
                    //PHASE 4 Beginn
                    // - -- - -- - -
                    if (startPhase == 1)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if(schwierigkeit>0)rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(MITMausgangsTextEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMmindestSchlüssellängeEve, true);
                        rolleEve.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        rolleBob.SpeicherInformationAb(MITMschlüssellängeEve, true);
                    }
                    else if (startPhase == 2)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(MITMausgangsTextEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMmindestSchlüssellängeEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        //-----------//
                        rolleAlice.SpeicherInformationAb(MITMschlüsselbits1Alice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(MITMpolschataAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(MITMphotonenAlice, true);
                        rolleEve.SpeicherInformationAb(MITMpolschataAliceEve, true);                     
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits1AliceEve, true);
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits1EveBob, true);
                        rolleEve.SpeicherInformationAb(MITMpolschataEveBob, true);
                        rolleEve.SpeicherInformationAb(MITMphotonenEveBob, true);
                        rolleBob.SpeicherInformationAb(MITMpolschataBob, true);
                        rolleBob.SpeicherInformationAb(MITMschlüsselbits1Bob, true);
                    }
                    else if (startPhase == 3)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(MITMausgangsTextEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMmindestSchlüssellängeEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        //-----------//
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(MITMschlüsselbits1Alice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMpolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMphotonenAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataAliceEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMschlüsselbits1AliceEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMschlüsselbits1EveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataEveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMphotonenEveBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMpolschataBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(MITMschlüsselbits1Bob, true);
                        //-----------//
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMpolschataAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataDifferenzAliceEve, true);
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits2AliceEve, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMpolschataDifferenzAliceEve, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(MITMpolschataEveBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMpolschataDifferenzBob, true);
                        rolleBob.SpeicherInformationAb(MITMschlüsselbits2Bob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataDifferenzBob, true);
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits2EveBob, true);
                        rolleAlice.SpeicherInformationAb(MITMschlüsselbits2Alice, true);
                    }
                    else if (startPhase == 4)
                    {
                        rolleAlice.SpeicherInformationAb(ausgangsTextAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(mindestSchlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(schlüssellängeAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(schlüssellängeAlice, true);
                        rolleEve.SpeicherInformationAb(MITMausgangsTextEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMmindestSchlüssellängeEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMschlüssellängeEve, true);
                        //-----------//
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(MITMschlüsselbits1Alice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMpolschataAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMphotonenAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataAliceEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMschlüsselbits1AliceEve, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMschlüsselbits1EveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataEveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMphotonenEveBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMpolschataBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(MITMschlüsselbits1Bob, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataAlice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataDifferenzAliceEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMschlüsselbits2AliceEve, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMpolschataDifferenzAliceEve, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMpolschataEveBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMpolschataDifferenzBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMschlüsselbits2Bob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMpolschataDifferenzBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMschlüsselbits2EveBob, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMschlüsselbits2Alice, true);
                        //-----------//
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMprüfbitAnzahlAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMlängePrüfmaskeAlice, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMprüfmaskeAlice, true);
                        if (schwierigkeit > 0) rolleAlice.SpeicherInformationAb(MITMprüfbitsAlice, true);
                        rolleAlice.SpeicherInformationAb(MITMschlüsselbits3Alice, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMprüfmaskeAlice, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMprüfbitsAliceEve, true);
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits3AliceEve, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMprüfbitsAliceEve, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMprüfbitAnzahlEveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMlängePrüfmaskeEveBob, true);
                        if (schwierigkeit > 1) rolleEve.SpeicherInformationAb(MITMprüfmaskeEveBob, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMprüfbitsEveBob, true);
                        rolleEve.SpeicherInformationAb(MITMschlüsselbits3EveBob, true);
                        if (schwierigkeit > 1) rolleBob.SpeicherInformationAb(MITMprüfmaskeEveBob, true);
                        if (schwierigkeit > 0) rolleBob.SpeicherInformationAb(MITMprüfbitsBob, true);
                        rolleBob.SpeicherInformationAb(MITMschlüsselbits3Bob, true);
                        if (schwierigkeit > 0) rolleEve.SpeicherInformationAb(MITMprüfbitsBob, true);
                        rolleEve.SpeicherInformationAb(MITMprüfbitsDifferenzEve, true);
                        if (schwierigkeit > 1) rolleAlice.SpeicherInformationAb(MITMprüfbitsAliceEve, true);
                        rolleAlice.SpeicherInformationAb(MITMprüfbitsDifferenzAlice, true);
                    }
                    break;
            }
            return seed;
        }
    }
}
