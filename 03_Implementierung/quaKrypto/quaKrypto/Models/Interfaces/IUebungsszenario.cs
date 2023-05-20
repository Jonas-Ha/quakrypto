// **********************************************************
// File: VarianteNormalerAblauf.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Classes;

namespace quaKrypto.Models.Interfaces
{
    public interface IUebungsszenario
    {
        public List<Rolle> Rollen
        { get; set; }

        public Enums.SchwierigkeitsgradEnum Schwierigkeitsgrad
        { get; set; }

        public IVariante Variante 
        { get; set; }

        public uint StartPhase
        { get; set; }

        public uint EndPhase
        { get; set; }

        public Uebertragungskanal Uebertragungskanal
        { get; set; }

        public Aufzeichnung Aufzeichnung
        { get; set; }

        public void VeroeffentlicheLobby();
        public bool RolleHinzufuegen(Rolle rolle);
        public bool RolleEntfernen(Rolle rolle);
        public void NaechsterZug();
        public void ErzeugeProtokoll();
    }
}
