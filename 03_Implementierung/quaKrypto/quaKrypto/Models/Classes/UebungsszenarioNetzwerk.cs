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
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

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
            if (host)
            {
                NetzwerkHost.Ubungsszenario = this;
            }
            else
            {
                NetzwerkClient.Ubungsszenario = this;
            }
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
                if(eigeneRolle)eigeneRollen.Add(rolle.RolleTyp);
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

                NetzwerkHost.StarteUebungsszenario(aktRolle);

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
                if (variante.AktuellePhase > endPhase)
                {
                    Beenden();
                    return;
                }
                else if (!eigeneRollen.Contains(aktRolle))
                    NetzwerkHost.UebergebeKontrolle(aktRolle);
            }
            else
            {
                
                NetzwerkClient.BeendeZug(aktuelleRolle.handlungsschritte);
            }

            //Wird das darunter noch gebraucht?
            aktuelleRolle.handlungsschritte.Clear();

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

        public Information HandlungsschrittAusführenLassen(OperationsEnum operationsTyp, Information operand1, object operand2, string ergebnisInformationsName, RolleEnum ausFührer)
        {
            Handlungsschritt handlungsschritt = aktuelleRolle.ErzeugeHandlungsschritt(operationsTyp, operand1, operand2, ergebnisInformationsName, ausFührer);
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

        public void Beenden()
        {
            if (host)
            {
                NetzwerkHost.BeendeUebungsszenario();
            }
            else
            {
                NetzwerkClient.BeendeUebungsszenario();
            }
            beendet = true;
            PropertyHasChanged(nameof(beendet));
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
            for(int i = 0; i < Rollen.Count; i++)
            {
                if (Rollen[i].RolleTyp == nächsteRolle)
                {
                    aktuelleRolle = Rollen[i];
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
            Trace.Write("Ga");
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

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }

    }
}
