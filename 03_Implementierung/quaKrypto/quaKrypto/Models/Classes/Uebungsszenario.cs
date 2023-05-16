// **********************************************************
// File: Uebungsszenario.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    internal class Uebungsszenario
    {
        private List<Rolle> rollen;
        private Enums.SchwierigkeitsgradEnum schwierigkeitsgrad;
        // mit Gruppe für das Interface der Lobby abstimmen
        // private IVariante variante;
        private uint startPhase;
        private uint endPhase;
        private uint aktuellePhase;
        private Uebertragungskanal uebertragungskanal;
        private Aufzeichnung aufzeichnung;

        Uebungsszenario(List<Rolle> rollen, Enums.SchwierigkeitsgradEnum schwierigkeitsgrad, uint startPhase, uint endPhase)
        {
            this.rollen = rollen;
            this.schwierigkeitsgrad = schwierigkeitsgrad;
            this.startPhase = startPhase;
            this.endPhase = endPhase;
            this.aktuellePhase = startPhase;
            this.uebertragungskanal = new Uebertragungskanal();
            this.aufzeichnung = new Aufzeichnung();
        }

        // Klärung: Wie sieht die Kommunikation bzw. Befehle an welcher Stelle zur Rolle hingeführt? - Alexander Denner
        void BeendeZug()
        {
            
        }

        void ErzeugeProtokoll()
        {

        }
    }
}
