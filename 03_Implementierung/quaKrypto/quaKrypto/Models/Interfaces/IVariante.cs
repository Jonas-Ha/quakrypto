// **********************************************************
// File: IVariante.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public RolleEnum AktuelleRolle
        { get; }


        public string VariantenName
        { get; }

        public string ProtokollName 
        { get; }

        public List<RolleEnum> MoeglicheRollen
        { get; }

        public RolleEnum NaechsteRolle();
        public List<OperationsEnum> GebeHilfestellung(SchwierigkeitsgradEnum schwierigkeitsgrad);

        public void BerechneAktuellePhase(object? sender, NotifyCollectionChangedEventArgs e);
    }
}
