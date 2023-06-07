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

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioLokal : IUebungsszenario, INotifyPropertyChanged
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

        public UebungsszenarioLokal(SchwierigkeitsgradEnum schwierigkeitsgrad, IVariante variante, uint startPhase, uint endPhase, string name)
        {
            this.rollen = new List<Rolle>();
            this.schwierigkeitsgrad = schwierigkeitsgrad;
            this.variante = variante;
            this.startPhase = startPhase;
            this.endPhase = endPhase;
            this.uebertragungskanal = new Uebertragungskanal();
            this.aufzeichnung = new Aufzeichnung();
            this.name = name;
            this.beendet = false;
        }

        public ReadOnlyCollection<Rolle> Rollen => rollen.AsReadOnly();
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get { return schwierigkeitsgrad; } }
        public IVariante Variante { get { return variante; } }
        public uint StartPhase { get { return startPhase; } }
        public uint EndPhase { get { return endPhase; } }
        public Uebertragungskanal Uebertragungskanal { get { return uebertragungskanal; } }
        public Aufzeichnung Aufzeichnung { get { return aufzeichnung; } }
        public string Name { get { return name; } }
        public bool Beendet { get { return beendet; } }

        //Überprüft ob die Rolle bereits vergeben ist und falls nicht wird die Rolle hinzugefügt und gibt zurück ob die Rolle hinzugefügt
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
            if(verfügbar)
            {
                rollen.Add(rolle);
                return true;
            }
            return false;
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

        public void Starten()
        {
            RolleEnum aktRolle = Variante.NaechsteRolle();
            for (int i = 0; i < Rollen.Count; i++)
            {
                if (aktRolle == Rollen[i].RolleTyp)
                {
                    aktuelleRolle = Rollen[i];
                    break;
                }
            }
        }

        public void NaechsterZug()
        {
            List<Handlungsschritt> handlungsschritte = aktuelleRolle.handlungsschritte;
            //Die Handlungsschritte müssen hier noch überprüft werden um die Informationsablage auf den richtigen Stand zu bekommen
            Aufzeichnung.HaengeListeHandlungsschritteAn(handlungsschritte.ToList());
            aktuelleRolle.handlungsschritte.Clear();
            if(variante.AktuellePhase > endPhase) 
            { 
                Beenden();
                return; 
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
            PropertyHasChanged(nameof(Rolle));
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

        //Speichert eine Information in der Ablage der aktuellen Rolle
        public void SpeichereInformationenAb(Information information)
        {
            aktuelleRolle.SpeicherInformationAb(information);
        }

        public void LoescheInformation(uint informationID)
        {
            aktuelleRolle.LoescheInformation(informationID);
        }

        public void Beenden()
        {
            beendet = true;
            PropertyHasChanged(nameof(beendet));
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
