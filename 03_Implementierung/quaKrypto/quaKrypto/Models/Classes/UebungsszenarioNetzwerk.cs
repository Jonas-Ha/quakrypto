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
        public bool RolleHinzufuegen(Rolle rolle)
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

            //TODO: Netzwerkfunktion
            if (host)
            {
                NetzwerkHost.SendeRollenInformation();
            }
            else
            {
                NetzwerkClient.WaehleRolle(rolle.RolleTyp, rolle.Alias);
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

            //TODO: Netzwerkfunktion
            if (host)
            {
                //Welche Funktion
            }
            else
            {
                NetzwerkClient.GebeRolleFrei(rolle);
            }
        }

        public bool Starten()
        {
            //Geht nur wenn Host -> Host flag hinzufügen
            if (host)
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
                        break;
                    }
                }

                //TODO: Netzwerkfunktion
                //NetzwerkHost.StarteUebungsszeanrio();
                //Wenn man selber nicht am zug dann noch:
                //NetzwerkHost.UebergebeKontrolle(aktuelleRolle)
                return true;
            }

            return false;
        }

        public void NaechsterZug()
        {
            //TODO: Netzwerkfunktion
            if (host)
            {
                //??
            }
            else
            {
                NetzwerkClient.BeendeZug(aktuelleRolle.handlungsschritte);
            }

            //Wird das darunter noch gebraucht?
            aktuelleRolle.handlungsschritte.Clear();
            if (variante.AktuellePhase > endPhase)
            {
                //TODO: Netzwerkfunktion
                Beenden();
                return;
            }
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
            //TODO: NEtzwerkfunktion
        }

        public bool GebeBildschirmFrei(string Passwort)
        {
            return aktuelleRolle.BeginneZug(Passwort);
        }

        public Information HandlungsschrittAusführenLassen(Enums.OperationsEnum operationsTyp, Information operand1, object operand2, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
            Handlungsschritt handlungsschritt = aktuelleRolle.ErzeugeHandlungsschritt(operationsTyp, operand1, operand2, ergebnisInformationsName, rolle);
            Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);
            return handlungsschritt.Ergebnis;
        }

        public void SpeichereInformationenAb(Information information)
        {
            aktuelleRolle.SpeicherInformationAb(information);
        }

        public void LoescheInformation(int informationID)
        {
            aktuelleRolle.LoescheInformation(informationID);
        }

        public void Beenden()
        {
            beendet = true;
            PropertyHasChanged(nameof(beendet));
        }

        /**
         * Schnittstelle für Befehle von Netzwerkklasse Host
         */
        public void ZugWurdeBeendet(List<Handlungsschritt> handlungsschritte)
        {
            //TODO: Bestimmen der anderen Rolle (nicht man selbst und nicht aktive)
            if(Rollen.Count > 2) NetzwerkHost.SendeAufzeichnungsUpdate(null, handlungsschritte);
            foreach(Handlungsschritt handlungsschritt in handlungsschritte)
            {
                HandlungsschrittAusführenLassen(handlungsschritt.OperationsTyp, handlungsschritt.Operand1,
                    handlungsschritt.Operand2, handlungsschritt.ErgebnisName, handlungsschritt.Rolle);
                Aufzeichnung.HaengeHandlungsschrittAn(handlungsschritt);
                //Wann nachricht abspeichern ?
            }

            RolleEnum naechsteRolle = variante.NaechsteRolle();
            foreach (Rolle rolle in rollen)
            {
                if (rolle.RolleTyp == naechsteRolle)
                {
                    aktuelleRolle = rolle;
                    break;
                }
            }
            PropertyHasChanged(nameof(aktuelleRolle));
            //Property Changed Event muss dann checken ob host aktuelle rolle ist, und ggf. Kontrolle übergeben
        }


        public void UebungsszenarioWurdeBeendetHost()
        {
            throw new NotImplementedException();
        }

        /**
         * Schnittstelle für Befehle von  Netzwerkklasse Client
         */
        public void UebungsszenarioWurdeBeendetClient()
        {
            throw new NotImplementedException();
        }

        public void AufzeichnungUpdate(List<Handlungsschritt> handlungsschritte)
        {
            throw new NotImplementedException();
        }

        public void KontrolleErhalten()
        {
            throw new NotImplementedException();
        }

        public void UebungsszenarioWurdeGestartet()
        {
            throw new NotImplementedException();
        }

        public void NeueRollenInformation(Rolle? rolleAlice, Rolle? rolleBob, Rolle? rolleEve)
        {
            throw new NotImplementedException();
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }

    }
}
