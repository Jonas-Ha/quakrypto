// **********************************************************
// File: IVariante.cs
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
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Interfaces
{
    public interface IVariante
    {
        public uint AktuellePhase
        { get; }

        // Macht es überhaupt Sinn den Schwierigkeitsgrad abzuspeichern, wenn dieser schon in Uebungsszenario vorhanden ist? - Jonas Hammer
        public SchwierigkeitsgradEnum Schwierigkeitsgrad 
        { get; }

        public string VariantenName
        { get; }

        public List<RolleEnum> MoeglicheRollen
        { get; }

        // 1. Möglichkeit: Anpassen der Phasen!?
        // 2. Möglichkeit: Nach jedem Handlungsschritt wird eine Operation aufgerufen, die die aktuelle Phase berechnet.
        public RolleEnum NaechsteRolle();
        public List<OperationsEnum> GebeHilfestellung();  
    }
}
