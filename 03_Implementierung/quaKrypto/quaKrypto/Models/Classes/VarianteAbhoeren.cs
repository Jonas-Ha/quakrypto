// **********************************************************
// File: VarianteAbhoeren.cs
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
    public class VarianteAbhoeren : IVariante
    {
        private uint aktuellePhase;
        private SchwierigkeitsgradEnum schwierigkeitsgrad;
        private List<RolleEnum> moeglicheRollen = new List<RolleEnum>{ RolleEnum.Alice, RolleEnum.Bob, RolleEnum.Eve };

        public uint AktuellePhase
        {
            get { return aktuellePhase; }
            set { aktuellePhase = value; }
        }

        public SchwierigkeitsgradEnum Schwierigkeitsgrad
        {
            get { return schwierigkeitsgrad; }
        }

        public string VariantenName
        {
            get { return "Abhoeren"; }
        }

        public List<RolleEnum> MoeglicheRollen
        {
            get { return moeglicheRollen; }
        }

        public VarianteAbhoeren(uint startPhase, SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            this.aktuellePhase = startPhase;
            this.schwierigkeitsgrad = schwierigkeitsgrad;
        }

        public RolleEnum NaechsteRolle()
        {
            return 0;
        }

        public List<OperationsEnum> GebeHilfestellung()
        {
            return null;
        }
    }
}
