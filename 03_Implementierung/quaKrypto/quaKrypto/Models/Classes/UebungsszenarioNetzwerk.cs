// **********************************************************
// File: UebungsszenarioNetzwerk.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;
using quaKrypto.Services;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerk : IUebungsszenario, INotifyPropertyChanged
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
        private bool host;
        private List<RolleEnum> eigeneRollen = new();
        private bool hostHatGestartet = false;

        public UebungsszenarioNetzwerk(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase, string name, bool host)
        {
            this.rollen = new ObservableCollection<Rolle>();
            this.rollenActual = new ReadOnlyObservableCollection<Rolle>(rollen);
            this.schwierigkeitsgrad = schwierigkeitsgrad;
            this.variante = variante;
            this.startPhase = startPhase;
            this.endPhase = endPhase;
            this.uebertragungskanal = new Uebertragungskanal();
            this.aufzeichnung = new Aufzeichnung();
            this.aufzeichnung.Handlungsschritte.CollectionChanged += this.variante.BerechneAktuellePhase;
            this.name = name;
            this.host = host;
            if (host) NetzwerkHost.Ubungsszenario = this;
            else NetzwerkClient.Ubungsszenario = this;
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
        public bool HostHatGestartet { get { return hostHatGestartet; } set { hostHatGestartet = value; this.PropertyHasChanged(nameof(HostHatGestartet)); } }
        public bool Host => host;
        public bool RolleHinzufuegen(Rolle rolle, bool eigeneRolle)
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
                if (host)
                {
                    switch (rolle.RolleTyp)
                    {
                        case RolleEnum.Alice:
                            NetzwerkHost.AliceRolle = rolle;
                            break;
                        case RolleEnum.Bob:
                            NetzwerkHost.BobRolle = rolle;
                            break;
                        case RolleEnum.Eve:
                            NetzwerkHost.EveRolle = rolle;
                            break;
                    }
                    NetzwerkHost.SendeRollenInformation();
                }
                else
                {
                    NetzwerkClient.WaehleRolle(rolle.RolleTyp, rolle.Alias);
                }
                if (eigeneRolle) eigeneRollen.Add(rolle.RolleTyp);
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
                    if (host)
                    {
                        switch (rolle)
                        {
                            case RolleEnum.Alice:
                                NetzwerkHost.AliceRolle = null;
                                break;
                            case RolleEnum.Bob:
                                NetzwerkHost.BobRolle = null;
                                break;
                            case RolleEnum.Eve:
                                NetzwerkHost.EveRolle = null;
                                break;
                        }
                        NetzwerkHost.SendeRollenInformation();
                    }
                    else
                    {
                        NetzwerkClient.GebeRolleFrei(rolle);
                    }
                    _ = eigeneRollen.Remove(rolle);
                    this.PropertyHasChanged(nameof(Rollen));
                    break;
                }
            }
        }

        public bool Starten()
        {
            //Geht nur wenn Host -> Host flag hinzufügen
            if (host && eigeneRollen.Count != 0)
            {
                int seed = GeneriereInformationenFürRollen();

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

                RolleEnum aktRolle = Variante.NaechsteRolle();
                for (int i = 0; i < Rollen.Count; i++)
                {
                    if (aktRolle == Rollen[i].RolleTyp)
                    {
                        aktuelleRolle = Rollen[i];
                        PropertyHasChanged(nameof(AktuelleRolle));
                        break;
                    }
                }

                NetzwerkHost.StarteUebungsszenario(aktRolle, seed);

                if (!eigeneRollen.Contains(aktRolle))
                {
                    NetzwerkHost.UebergebeKontrolle(aktRolle);
                }
                return true;
            }

            return false;
        }

        public void NaechsterZug()
        {

            RolleEnum aktRolle = Variante.NaechsteRolle();
            if (host)
            {
                //Aufzeichnung wird an alle gesendet, da man Host ist.
                NetzwerkHost.SendeAufzeichnungsUpdate(aktuelleRolle.handlungsschritte);
                if (eigeneRollen.Count != 1)
                {
                    foreach (Handlungsschritt handlungsschritt in aktuelleRolle.handlungsschritte)
                    {
                        if (handlungsschritt.OperationsTyp == OperationsEnum.nachrichtSenden && (eigeneRollen.Contains(RolleEnum.Eve) || (handlungsschritt.Rolle == RolleEnum.Alice && eigeneRollen.Contains(RolleEnum.Bob)) || (handlungsschritt.Rolle == RolleEnum.Bob && eigeneRollen.Contains(RolleEnum.Alice))))
                        {
                            Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
                        }
                    }
                }

                if (!eigeneRollen.Contains(aktRolle))
                    NetzwerkHost.UebergebeKontrolle(aktRolle);
                else
                {
                    foreach (Rolle rolle in rollen)
                    {
                        if (rolle.RolleTyp == aktRolle)
                        {
                            aktuelleRolle = rolle;
                            break;
                        }
                    }
                    PropertyHasChanged(nameof(aktuelleRolle));
                }
            }
            else
            {
                NetzwerkClient.BeendeZug(aktuelleRolle.handlungsschritte);
            }

            //Wird das darunter noch gebraucht?
            aktuelleRolle.handlungsschritte.Clear();
            PrüfenSpielBeendet();
        }

        public bool GebeBildschirmFrei(string Passwort)
        {
            return aktuelleRolle.BeginneZug(Passwort);
        }

        public Information HandlungsschrittAusführenLassen(OperationsEnum operationsTyp, Information operand1, object operand2, string ergebnisInformationsName, RolleEnum ausFührer)
        {
            Handlungsschritt handlungsschritt = aktuelleRolle.ErzeugeHandlungsschritt(operationsTyp, operand1, operand2, ergebnisInformationsName, ausFührer);
            handlungsschritt.AktuellePhase = Variante.AktuellePhase;
            Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);
            return handlungsschritt.Ergebnis;
        }

        public void SpeichereInformationenAb(Information information)
        {
            aktuelleRolle.SpeicherInformationAb(information);
        }

        public void LoescheInformation(int informationsID)
        {
            aktuelleRolle.LoescheInformation(informationsID);
        }

        public void LoescheInformationAusUebertragungskanal(KanalEnum kanal, int informatonsID)
        {
            Uebertragungskanal.LoescheNachricht(kanal, informatonsID);
        }

        public void Beenden()
        {
            NetzwerkHost.BeendenErlaubt = false;
            beendet = true;
            PropertyHasChanged(nameof(Beendet));
            NetzwerkHost.BeendenErlaubt = true;
            if (host) NetzwerkHost.BeendeUebungsszenario();
            else NetzwerkClient.BeendeUebungsszenario();
        }

        /**
         * Schnittstelle für Befehle von Netzwerkklasse Host
         */
        public void ZugWurdeBeendet(List<Handlungsschritt> handlungsschritte)
        {
            //Bestimmen der anderen Rolle (nicht man selbst und nicht aktive)
            if (Variante.GetType() != typeof(VarianteNormalerAblauf) && eigeneRollen.Count == 1) NetzwerkHost.SendeAufzeichnungsUpdate(handlungsschritte, !eigeneRollen.Contains(RolleEnum.Alice) && aktuelleRolle.RolleTyp != RolleEnum.Alice ? RolleEnum.Alice : !eigeneRollen.Contains(RolleEnum.Bob) && aktuelleRolle.RolleTyp != RolleEnum.Bob ? RolleEnum.Bob : RolleEnum.Eve);
            foreach (Handlungsschritt handlungsschritt in handlungsschritte)
            {
                Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);

                //rollen.Where(rolle => rolle.RolleTyp == handlungsschritt.Rolle).First().BeginneZug("");
                //HandlungsschrittAusführenLassen(handlungsschritt.OperationsTyp, handlungsschritt.Operand1, handlungsschritt.Operand2, handlungsschritt.ErgebnisName, handlungsschritt.Rolle);
                if (handlungsschritt.OperationsTyp == OperationsEnum.nachrichtSenden && (eigeneRollen.Contains(RolleEnum.Eve) || (handlungsschritt.Rolle == RolleEnum.Alice && eigeneRollen.Contains(RolleEnum.Bob)) || (handlungsschritt.Rolle == RolleEnum.Bob && eigeneRollen.Contains(RolleEnum.Alice))))
                {
                    Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
                }
            }
            if (Beendet) return;
            RolleEnum naechsteRolle = variante.NaechsteRolle();

            if (!eigeneRollen.Contains(naechsteRolle))
                NetzwerkHost.UebergebeKontrolle(naechsteRolle);

            foreach (Rolle rolle in rollen)
            {
                if (rolle.RolleTyp == naechsteRolle)
                {
                    aktuelleRolle = rolle;
                    break;
                }
            }
            PropertyHasChanged(nameof(aktuelleRolle));
            PrüfenSpielBeendet();
        }

        /**
         * Schnittstelle für Befehle von  Netzwerkklasse Client
         */

        public void AufzeichnungUpdate(List<Handlungsschritt> handlungsschritte)
        {
            foreach (Handlungsschritt handlungsschritt in handlungsschritte)
            {
                Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);
                //rollen.Where(rolle => rolle.RolleTyp == handlungsschritt.Rolle).First().BeginneZug("");
                //HandlungsschrittAusführenLassen(handlungsschritt.OperationsTyp, handlungsschritt.Operand1, handlungsschritt.Operand2, handlungsschritt.ErgebnisName, handlungsschritt.Rolle);
                if (handlungsschritt.OperationsTyp == OperationsEnum.nachrichtSenden && (eigeneRollen.Contains(RolleEnum.Eve) || (handlungsschritt.Rolle == RolleEnum.Alice && eigeneRollen.Contains(RolleEnum.Bob)) || (handlungsschritt.Rolle == RolleEnum.Bob && eigeneRollen.Contains(RolleEnum.Alice))))
                {
                    Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
                }
            }
        }

        public void KontrolleErhalten(RolleEnum nächsteRolle)
        {
            //Lobbyscreenview muss Bildschirm freigeben und Passwort eingeben lassen.
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (Rollen[i].RolleTyp == nächsteRolle)
                {
                    aktuelleRolle = Rollen[i];
                    PropertyHasChanged(nameof(aktuelleRolle));
                    break;
                }
            }
        }

        public void UebungsszenarioWurdeGestartet(RolleEnum startRolle)
        {
            //Views müssen auf Spiel umschalten und den WarteBildschirm anzeigen

            for (int i = 0; i < Rollen.Count; i++)
            {
                if (Rollen[i].RolleTyp == startRolle)
                {
                    aktuelleRolle = Rollen[i];
                    break;
                }
            }
            HostHatGestartet = true;
        }


        public void NeueRollenInformation(Rolle? rolleAlice, Rolle? rolleBob, Rolle? rolleEve)
        {
            Rolle? rolle = rollen.Where(r => r.RolleTyp == RolleEnum.Alice).FirstOrDefault();
            if (rolleAlice != null && (rolle == null || rolle == default(Rolle))) { rollen.Add(rolleAlice); this.PropertyHasChanged(nameof(Rollen)); }
            else if (rolleAlice == null && (rolle != null && rolle != default(Rolle))) rollen.Remove(rolle);

            rolle = rollen.Where(r => r.RolleTyp == RolleEnum.Bob).FirstOrDefault();
            if (rolleBob != null && (rolle == null || rolle == default(Rolle))) { rollen.Add(rolleBob); this.PropertyHasChanged(nameof(Rollen)); }
            else if (rolleBob == null && (rolle != null && rolle != default(Rolle))) rollen.Remove(rolle);

            rolle = rollen.Where(r => r.RolleTyp == RolleEnum.Eve).FirstOrDefault();
            if (rolleEve != null && (rolle == null || rolle == default(Rolle))) { rollen.Add(rolleEve); this.PropertyHasChanged(nameof(Rollen)); }
            else if (rolleEve == null && (rolle != null && rolle != default(Rolle))) rollen.Remove(rolle);
        }

        private void VarianteChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (host)
            {
                if (variante.AktuellePhase >= endPhase)
                {
                    Beenden();
                    return;
                }
            }
        }
        private void PrüfenSpielBeendet()
        {
            if (host)
            {
                if (variante.AktuellePhase >= endPhase)
                {
                    Beenden();
                    return;
                }
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
            string ausgangsTextString = StandardTexte.BekommeZufälligenText(seed);

            Operationen operationen = new(seed);

            int zähler = -1;

            Information ausgangsText = new(zähler--, "Geheimtext", InformationsEnum.asciiText, ausgangsTextString);
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
                    //PHASE 2 ENDE
                    Information prüfbitAnzahl = new(zähler--, "Anzahl der Prüfbits", InformationsEnum.zahl, ((bool[])schlüsselbits2Alice.InformationsInhalt).Length / 10);
                    Information längePrüfmaske = new(zähler--, "Länge Prüfmaske", InformationsEnum.zahl, ((bool[])schlüsselbits2Alice.InformationsInhalt).Length);
                    Information prüfmaske = operationen.BitmaskeGenerieren(zähler--, längePrüfmaske, prüfbitAnzahl, "Prüfmaske");
                    Information prüfbitsAlice = operationen.BitsStreichen(zähler--, schlüsselbits2Alice, operationen.BitfolgeNegieren(zähler--, prüfmaske, null, ""), "Prüfbits");
                    Information schlüsselbits3Alice = operationen.BitsStreichen(zähler--, schlüsselbits2Alice, prüfmaske, "Schlüsselbits - Final");

                    Information prüfbitsBob = operationen.BitsStreichen(zähler--, schlüsselbits2Bob, operationen.BitfolgeNegieren(zähler--, prüfmaske, null, ""), "Prüfbits");
                    Information schlüsselbits3Bob = operationen.BitsStreichen(zähler--, schlüsselbits2Bob, prüfmaske, "Schlüsselbits - Final");

                    Information prüfbitsDifferenzAlice = operationen.BitfolgenVergleichen(zähler--, prüfbitsAlice, prüfbitsBob, "Unterschied Prüfbits");
                    //PHASE 3 ENDE
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
            return seed;
        }

    }
}
