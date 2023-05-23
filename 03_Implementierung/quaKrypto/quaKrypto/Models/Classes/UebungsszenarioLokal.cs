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

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioLokal : IUebungsszenario
    {
        private List<Rolle> rollen;
        private Rolle aktuelleRolle;
        private SchwierigkeitsgradEnum schwierigkeitsgrad;
        private IVariante variante;
        private uint startPhase;
        private uint endPhase;
        private Uebertragungskanal uebertragungskanal;
        private Aufzeichnung aufzeichnung;

        public UebungsszenarioLokal(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase)
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

        public List<Rolle> Rollen
        {
            get { return rollen; }
            set { rollen = value; }
        }

        public Rolle AktuelleRolle
        {
            get { return aktuelleRolle; }
            set { aktuelleRolle = value; }
        }

        public Enums.SchwierigkeitsgradEnum Schwierigkeitsgrad
        {
            get { return schwierigkeitsgrad; }
            set { schwierigkeitsgrad = value; }
        }

        public IVariante Variante
        {
            get { return variante; }
            set { variante = value; }
        }

        public uint StartPhase
        {
            get { return startPhase; }
            set { startPhase = value; }
        }

        public uint EndPhase
        {
            get { return endPhase; }
            set { endPhase = value; }
        }

        public Uebertragungskanal Uebertragungskanal
        {
            get { return uebertragungskanal; }
            set { uebertragungskanal = value; }
        }

        public Aufzeichnung Aufzeichnung
        {
            get { return aufzeichnung; }
            set { aufzeichnung = value; }
        }

        // wird erstmal nicht implementiert, da ???
        public void VeroeffentlicheLobby()
        {

        }

        public bool RolleHinzufuegen(Rolle rolle)
        {
            var moeglicheRollen = variante.MoeglicheRollen;

            if (moeglicheRollen.Contains(rolle.RolleTyp) && !this.rollen.Contains(rolle))
            {
                this.rollen.Add(rolle);
                return true;
            }
            else { return false; }
        }

        public bool RolleEntfernen(Rolle rolle)
        {
            if (this.rollen.Contains(rolle))
            {
                this.rollen.Remove(rolle);
                return true;
            }
            else { return false; }
        }

        public bool NaechsterZug(String passwort)
        {
            if (this.aktuelleRolle != null)
            {
                RolleEnum naechsteRolle = variante.NaechsteRolle();
                Rolle pruefendeRolle = rollen.Find(Rolle => Rolle.RolleTyp == naechsteRolle);
                bool check = pruefendeRolle.BeginneZug(passwort);
                if (check)
                {
                    aktuelleRolle = pruefendeRolle;
                    return true;
                }
                return false;
            }
            return false; //TODO WAS HIER PASSIEREN MUSS - Denner 23.05.23
        }

        // Was ist damit gemeint, soll das für das Anzeigen des Protokolls sein?
        public void ErzeugeProtokoll()
        {

        }
    }
}
