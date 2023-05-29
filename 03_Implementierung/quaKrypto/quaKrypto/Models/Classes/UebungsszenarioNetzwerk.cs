// **********************************************************
// File: UebungsszenarioNetzwerk.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerk : IUebungsszenario
    {
        private List<Rolle> rollen;
        private Rolle aktuelleRolle;
        private SchwierigkeitsgradEnum schwierigkeitsgrad;
        private IVariante variante;
        private uint startPhase;
        private uint endPhase;
        private Uebertragungskanal uebertragungskanal;
        private Aufzeichnung aufzeichnung;

        public UebungsszenarioNetzwerk(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase)
        {
            this.rollen = new List<Rolle>();
            this.aktuelleRolle = null;
            this.schwierigkeitsgrad = schwierigkeitsgrad;
            this.variante = variante;
            this.startPhase = startPhase;
            this.endPhase = endPhase;
            this.uebertragungskanal = new Uebertragungskanal();
            this.aufzeichnung = new Aufzeichnung();
        }

        public List<Rolle> Rollen { get; }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get; }
        public IVariante Variante { get; }
        public uint StartPhase { get; }
        public uint EndPhase { get; }
        public Uebertragungskanal Uebertragungskanal { get; }
        public Aufzeichnung Aufzeichnung { get; }
        public string Name { get; }
        public bool RolleHinzufuegen(Rolle rolle)
        {
            throw new NotImplementedException();
        }

        public void GebeRolleFrei(RolleEnum rolle)
        {
            throw new NotImplementedException();
        }

        public void Starten()
        {
            throw new NotImplementedException();
        }

        public bool NaechsterZug()
        {
            throw new NotImplementedException();
        }

        public bool GebeBildschirmFrei(string Passwort)
        {
            throw new NotImplementedException();
        }

        public Information HandlungsschrittAusführenLassen(Handlungsschritt handlungsschritt)
        {
            throw new NotImplementedException();
        }

        public void SpeichereInformationenAb(uint informationID)
        {
            throw new NotImplementedException();
        }

        public void LoescheInformation(uint informationID)
        {
            throw new NotImplementedException();
        }

        public void Beenden()
        {
            throw new NotImplementedException();
        }

        /**
         * Schnittstelle für Befehle von Netzwerkklasse Host
         */
        public void ZugWurdeBeendet(List<Handlungsschritt> handlungsschritte)
        {
            throw new NotImplementedException();
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

    }
}
