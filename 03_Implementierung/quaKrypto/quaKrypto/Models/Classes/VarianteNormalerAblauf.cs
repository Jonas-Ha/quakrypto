// **********************************************************
// File: VarianteNormalerAblauf.cs
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
using quaKrypto.Models.Interfaces;
using quaKrypto.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace quaKrypto.Models.Classes
{
    public class VarianteNormalerAblauf : IVariante, INotifyPropertyChanged
    {
        private uint aktuellePhase;
        public readonly IList<RolleEnum> moeglicheRollen = new ReadOnlyCollection<RolleEnum>
            (new List<RolleEnum> { RolleEnum.Alice, RolleEnum.Bob });

        private RolleEnum aktuelleRolle;

        public event PropertyChangedEventHandler? PropertyChanged;

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
            get { return "Normaler Ablauf"; }
        }

        public string ProtokollName
        {
            get { return "BB84"; }
        }

        public IList<RolleEnum> MoeglicheRollen
        {
            get { return moeglicheRollen; }
        }

        public VarianteNormalerAblauf(uint startPhase) 
        {
            this.aktuellePhase = startPhase;

            // Alice fängt jedesmal in einer Phase an, daher ist Bob immer als letztes dran gewesen
            this.aktuelleRolle = RolleEnum.Bob;
        }
        public void AktuelleRolleSetzen(RolleEnum rolle)
        {
            aktuelleRolle = rolle;
        }

        public RolleEnum NaechsteRolle()
        {
            if (this.aktuelleRolle == RolleEnum.Alice)
            {
                this.aktuelleRolle = RolleEnum.Bob;
                return RolleEnum.Bob;
            }
            else
            {
                this.aktuelleRolle = RolleEnum.Alice;
                return RolleEnum.Alice;
            }
        }

        public void BerechneAktuellePhase(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null && e.NewItems!.Count == 1)
                {
                    Handlungsschritt neusterHandlungsschritt = (Handlungsschritt) e.NewItems[0]!;
                    if (aktuellePhase is 0 or 1 && neusterHandlungsschritt.OperationsTyp is OperationsEnum.zugBeenden &&
                        neusterHandlungsschritt.Rolle == RolleEnum.Bob) { aktuellePhase += 1; PropertyHasChanged(nameof(aktuellePhase)); }
                    if (aktuellePhase == 2 && neusterHandlungsschritt.OperationsTyp == OperationsEnum.bitsStreichen &&
                        neusterHandlungsschritt.Rolle == RolleEnum.Alice) { aktuellePhase += 1; PropertyHasChanged(nameof(aktuellePhase)); }
                    if (aktuellePhase == 3 && neusterHandlungsschritt.OperationsTyp == OperationsEnum.bitfolgenVergleichen &&
                        neusterHandlungsschritt.Rolle == RolleEnum.Alice) { aktuellePhase += 1; PropertyHasChanged(nameof(aktuellePhase)); }
                    if (aktuellePhase == 4 &&
                        neusterHandlungsschritt.OperationsTyp == OperationsEnum.textEntschluesseln &&
                        neusterHandlungsschritt.Rolle == RolleEnum.Bob) { aktuellePhase += 1; PropertyHasChanged(nameof(aktuellePhase)); }
                }
            }
        }

        public List<OperationsEnum> GebeHilfestellung(SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            switch (schwierigkeitsgrad)
            {
                case SchwierigkeitsgradEnum.Leicht: return GebeHilfestellungLeicht();
                case SchwierigkeitsgradEnum.Mittel: return GebeHilfestellungMittel();
                case SchwierigkeitsgradEnum.Schwer: return new List<OperationsEnum>();
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
            List<OperationsEnum> op = Enum.GetValues(typeof(OperationsEnum)).Cast<OperationsEnum>().ToList();

            switch (this.aktuellePhase)
            {
                case 0:
                    op.Remove(OperationsEnum.bitfolgeNegieren);
                    op.Remove(OperationsEnum.bitfolgenVergleichen);
                    op.Remove(OperationsEnum.bitsFreiBearbeiten);
                    op.Remove(OperationsEnum.bitsStreichen);
                    op.Remove(OperationsEnum.polschataVergleichen);
                    op.Remove(OperationsEnum.textEntschluesseln);
                    op.Remove(OperationsEnum.textVerschluesseln);
                    break;
                case 1:
                    op.Remove(OperationsEnum.bitfolgeNegieren);
                    op.Remove(OperationsEnum.bitfolgenVergleichen);
                    op.Remove(OperationsEnum.bitsFreiBearbeiten);
                    op.Remove(OperationsEnum.bitsStreichen);
                    op.Remove(OperationsEnum.polschataVergleichen);
                    op.Remove(OperationsEnum.textEntschluesseln);
                    op.Remove(OperationsEnum.textVerschluesseln);
                    break;
                case 2:
                    op.Remove(OperationsEnum.bitfolgeGenerierenAngabe);
                    op.Remove(OperationsEnum.bitmaskeGenerieren);
                    op.Remove(OperationsEnum.bitfolgeGenerierenZahl);
                    op.Remove(OperationsEnum.photonenGenerieren);
                    op.Remove(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Remove(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Remove(OperationsEnum.textGenerieren);
                    op.Remove(OperationsEnum.zahlGenerieren);
                    op.Remove(OperationsEnum.textEntschluesseln);
                    op.Remove(OperationsEnum.textVerschluesseln);
                    break;
                case 3:
                    op.Remove(OperationsEnum.textEntschluesseln);
                    op.Remove(OperationsEnum.textVerschluesseln);
                    break;
                case 4:
                    op.Remove(OperationsEnum.bitfolgeGenerierenAngabe);
                    op.Remove(OperationsEnum.bitmaskeGenerieren);
                    op.Remove(OperationsEnum.bitfolgeGenerierenZahl);
                    op.Remove(OperationsEnum.photonenGenerieren);
                    op.Remove(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Remove(OperationsEnum.polarisationsschemataGenerierenZahl);
                    break;
            }

            return op;
        }

        private void PropertyHasChanged(string nameOfProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
