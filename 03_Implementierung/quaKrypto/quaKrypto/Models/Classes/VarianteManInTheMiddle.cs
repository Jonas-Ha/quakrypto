// **********************************************************
// File: ManInTheMiddle.cs
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
    public class VarianteManInTheMiddle : IVariante
    {
        private uint aktuellePhase;
        private SchwierigkeitsgradEnum schwierigkeitsgrad;
        private List<RolleEnum> moeglicheRollen = new List<RolleEnum> { RolleEnum.Alice, RolleEnum.Bob, RolleEnum.Eve };

        private RolleEnum vorherigeRolle;
        private RolleEnum aktuelleRolle;
        private bool warAliceInPhaseAktiv;

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
            get { return "Normaler Ablauf"; }
        }

        public List<RolleEnum> MoeglicheRollen
        {
            get { return moeglicheRollen; }
        }

        public VarianteManInTheMiddle(uint startPhase, SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            this.aktuellePhase = startPhase;
            this.schwierigkeitsgrad = schwierigkeitsgrad;

            // Alice fängt jedesmal in einer Phase an, daher ist Bob immer als letztes dran gewesen
            this.aktuelleRolle = RolleEnum.Bob;
            this.warAliceInPhaseAktiv = false;
        }

        public RolleEnum NaechsteRolle()
        {
            if (aktuelleRolle == RolleEnum.Alice) return RolleEnum.Eve;
            else if (aktuelleRolle == RolleEnum.Bob) return RolleEnum.Eve;
            else if (vorherigeRolle == RolleEnum.Alice) return RolleEnum.Bob;
            else return RolleEnum.Alice;
        }

        public void AktuelleRolleAktualisieren(RolleEnum aktuelleRolle)
        {
            this.vorherigeRolle = this.aktuelleRolle;
            this.aktuelleRolle = aktuelleRolle;
        }

        public List<OperationsEnum> GebeHilfestellung()
        {
            switch (this.schwierigkeitsgrad)
            {
                case SchwierigkeitsgradEnum.leicht: return GebeHilfestellungLeicht();
                case SchwierigkeitsgradEnum.mittel: return GebeHilfestellungMittel();
                case SchwierigkeitsgradEnum.schwer: return new List<OperationsEnum>();
                default: return new List<OperationsEnum>();
            }
        }

        private List<OperationsEnum> GebeHilfestellungLeicht()
        {
            List<OperationsEnum> op = new List<OperationsEnum>();

            if (this.aktuellePhase == 0)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.textGenerieren);
                    op.Add(OperationsEnum.zahlGenerieren);
                }
            }
            else if (this.aktuellePhase == 1)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.bitfolgeGenerierenAngabe);
                    op.Add(OperationsEnum.bitfolgeGenerierenZahl);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenGenerieren);
                }
                else
                {
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenZuBitfolge);
                }
            }
            else if (this.aktuellePhase == 2)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.bitsStreichen);
                }
                else
                {
                    op.Add(OperationsEnum.polschataVergleichen);
                    op.Add(OperationsEnum.bitsStreichen);
                }
            }
            else if (this.aktuellePhase == 3)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.zahlGenerieren);
                    op.Add(OperationsEnum.bitmaskeGenerieren);
                    op.Add(OperationsEnum.bitsStreichen);
                    op.Add(OperationsEnum.bitfolgenVergleichen);
                }
                else
                {
                    op.Add(OperationsEnum.bitsStreichen);
                }
            }
            else if (this.aktuellePhase == 4)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.textVerschluesseln);
                }
                else
                {
                    op.Add(OperationsEnum.textEntschluesseln);
                }
            }

            return op;
        }

        private List<OperationsEnum> GebeHilfestellungMittel()
        {
            List<OperationsEnum> op = new List<OperationsEnum>();

            if (this.aktuellePhase == 0)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.textGenerieren);
                    op.Add(OperationsEnum.zahlGenerieren);
                }
            }
            else if (this.aktuellePhase == 1)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.bitfolgeGenerierenAngabe);
                    op.Add(OperationsEnum.bitfolgeGenerierenZahl);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenGenerieren);
                }
                else
                {
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenZuBitfolge);
                }
            }
            else if (this.aktuellePhase == 2)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.bitsStreichen);
                }
                else
                {
                    op.Add(OperationsEnum.polschataVergleichen);
                    op.Add(OperationsEnum.bitsStreichen);
                }
            }
            else if (this.aktuellePhase == 3)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.zahlGenerieren);
                    op.Add(OperationsEnum.bitmaskeGenerieren);
                    op.Add(OperationsEnum.bitsStreichen);
                    op.Add(OperationsEnum.bitfolgenVergleichen);
                }
                else
                {
                    op.Add(OperationsEnum.bitsStreichen);
                }
            }
            else if (this.aktuellePhase == 4)
            {
                if (this.aktuelleRolle == RolleEnum.Alice)
                {
                    op.Add(OperationsEnum.textVerschluesseln);
                }
                else
                {
                    op.Add(OperationsEnum.textEntschluesseln);
                }
            }

            return op;
        }
    }
}
