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
using System.Collections.Specialized;

namespace quaKrypto.Models.Classes
{
    public class VarianteManInTheMiddle : IVariante
    {
        private uint aktuellePhase;
        private List<RolleEnum> moeglicheRollen = new List<RolleEnum> { RolleEnum.Alice, RolleEnum.Bob };

        private RolleEnum vorherigeRolle;
        private RolleEnum aktuelleRolle;

        public uint AktuellePhase
        {
            get { return aktuellePhase; }
        }

        public RolleEnum AktuelleRolle
        {
            get { return aktuelleRolle; }
        }

        public static string VariantenName
        {
            get { return "Man-In-The-Middle"; }
        }

        public string ProtokollName
        {
            get { return "BB84"; }
        }

        public List<RolleEnum> MoeglicheRollen
        {
            get { return moeglicheRollen; }
        }

        public VarianteManInTheMiddle(uint startPhase)
        {
            this.aktuellePhase = startPhase;

            // Alice fängt jedesmal in einer Phase an, daher ist Bob immer als letztes dran gewesen
            vorherigeRolle = RolleEnum.Bob;
            this.aktuelleRolle = RolleEnum.Eve;

        }

        public RolleEnum NaechsteRolle()
        {
            if (this.aktuelleRolle == RolleEnum.Eve)
            {
                if (vorherigeRolle == RolleEnum.Bob) this.aktuelleRolle = RolleEnum.Alice;
                else if (vorherigeRolle == RolleEnum.Alice) this.aktuelleRolle = RolleEnum.Bob;
            }
            else this.aktuelleRolle = RolleEnum.Eve;

            return this.aktuelleRolle;
        }

        public void BerechneAktuellePhase(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null && e.NewItems!.Count == 1)
                {
                    Handlungsschritt neusterHandlungsschritt = (Handlungsschritt)e.NewItems[0]!;
                    if (aktuellePhase is 0 or 1 && neusterHandlungsschritt.OperationsTyp is OperationsEnum.zugBeenden &&
                        aktuelleRolle == RolleEnum.Eve) aktuellePhase += 1;
                    if (aktuellePhase == 2 && neusterHandlungsschritt.OperationsTyp == OperationsEnum.bitsStreichen &&
                        aktuelleRolle == RolleEnum.Alice) aktuellePhase += 1;
                    if (aktuellePhase == 3 && neusterHandlungsschritt.OperationsTyp == OperationsEnum.bitfolgenVergleichen && aktuelleRolle == RolleEnum.Alice) aktuellePhase += 1;
                    if (aktuellePhase == 4 &&
                        neusterHandlungsschritt.OperationsTyp == OperationsEnum.textEntschluesseln &&
                        aktuelleRolle == RolleEnum.Bob) aktuellePhase += 1;
                }
            }
        }

        public List<OperationsEnum> GebeHilfestellung(SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            switch (schwierigkeitsgrad)
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
