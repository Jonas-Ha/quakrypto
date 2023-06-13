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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerk : IUebungsszenario, INotifyPropertyChanged
    {
        private List<Rolle> rollen;
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
        private RolleEnum? eigeneRolle;

        public UebungsszenarioNetzwerk(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase, string name, bool host)
        {
            this.rollen = new List<Rolle>();
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

        public ReadOnlyCollection<Rolle> Rollen { get; }
        public Rolle AktuelleRolle { get { return aktuelleRolle; } }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get; }
        public IVariante Variante { get; }
        public uint StartPhase { get; }
        public uint EndPhase { get; }
        public Uebertragungskanal Uebertragungskanal { get; }
        public Aufzeichnung Aufzeichnung { get; }
        public string Name { get; }
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
                this.eigeneRolle = rolle.RolleTyp;
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
                    if (rolle == eigeneRolle) eigeneRolle = null;
                    break;
                }
            }
        }

        public bool Starten()
        {
            //Geht nur wenn Host -> Host flag hinzufügen
            if (host && eigeneRolle != null)
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

                NetzwerkHost.StarteUebungsszenario();

                if (aktRolle != eigeneRolle)
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
                if (variante.AktuellePhase > endPhase)
                {
                    Beenden();
                    return;
                }
                else
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
            if (Rollen.Count > 2) NetzwerkHost.SendeAufzeichnungsUpdate(handlungsschritte, eigeneRolle != RolleEnum.Alice && aktuelleRolle.RolleTyp != RolleEnum.Alice ? RolleEnum.Alice : eigeneRolle != RolleEnum.Bob && aktuelleRolle.RolleTyp != RolleEnum.Bob ? RolleEnum.Bob : RolleEnum.Eve);
            foreach (Handlungsschritt handlungsschritt in handlungsschritte)
            {
                HandlungsschrittAusführenLassen(handlungsschritt.OperationsTyp, handlungsschritt.Operand1, handlungsschritt.Operand2, handlungsschritt.ErgebnisName, handlungsschritt.Rolle);
                if (handlungsschritt.OperationsTyp == OperationsEnum.nachrichtSenden && (eigeneRolle == RolleEnum.Eve || (handlungsschritt.Rolle == RolleEnum.Alice && eigeneRolle == RolleEnum.Bob) || (handlungsschritt.Rolle == RolleEnum.Bob && eigeneRolle == RolleEnum.Alice)))
                {
                    Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
                }
            }

            RolleEnum naechsteRolle = variante.NaechsteRolle();

            if (naechsteRolle != eigeneRolle)
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
                HandlungsschrittAusführenLassen(handlungsschritt.OperationsTyp, handlungsschritt.Operand1, handlungsschritt.Operand2, handlungsschritt.ErgebnisName, handlungsschritt.Rolle);
                if (handlungsschritt.OperationsTyp == OperationsEnum.nachrichtSenden && (eigeneRolle == RolleEnum.Eve || (handlungsschritt.Rolle == RolleEnum.Alice && eigeneRolle == RolleEnum.Bob) || (handlungsschritt.Rolle == RolleEnum.Bob && eigeneRolle == RolleEnum.Alice)))
                {
                    Uebertragungskanal.SpeicherNachrichtAb(handlungsschritt.Ergebnis);
                }
            }
        }

        public void KontrolleErhalten()
        {
            //Lobbyscreenview muss Bildschirm freigeben und Passwort eingeben lassen.
            throw new NotImplementedException();
        }

        public void UebungsszenarioWurdeGestartet()
        {
            //Views müssen auf Spiel umschalten und den WarteBildschirm anzeigen
            throw new NotImplementedException();
        }

        public void NeueRollenInformation(Rolle? rolleAlice, Rolle? rolleBob, Rolle? rolleEve)
        {
            //LobbySceenView muss die aktualisierten Rollen anzeigen
            //Rollen müssen irgendwo hinzugefügt/entfernt werden
            throw new NotImplementedException();
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }

    }
}
