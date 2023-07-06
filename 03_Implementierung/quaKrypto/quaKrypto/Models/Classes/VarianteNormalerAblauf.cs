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
using quaKrypto.Models.Interfaces;
using quaKrypto.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace quaKrypto.Models.Classes
{
    public class VarianteNormalerAblauf : IVariante
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        private uint _aktuellePhase;

        public uint AktuellePhase
        {
            get { return _aktuellePhase; }
        }

        public RolleEnum AktuelleRolle
        {
            get;
            set;
        }

        public static string VariantenName
        {
            get { return "Normaler Ablauf"; }
        }

        public string ProtokollName
        {
            get { return "BB84"; }
        }

        //Im Normalen Ablauf sind nur ALice und Bob Teilnehmer
        public IList<RolleEnum> MöglicheRollen { get; } = new ReadOnlyCollection<RolleEnum>
            (new List<RolleEnum> { RolleEnum.Alice, RolleEnum.Bob }); 

        public VarianteNormalerAblauf(uint startPhase) 
        {
            _aktuellePhase = startPhase;

            //Alice beginnt in jeder Phase an, daher ist Bob immer als letztes dran gewesen
           AktuelleRolle = RolleEnum.Bob;
        }

        
        public RolleEnum NächsteRolle()
        {
            //Alice und Bob wechseln sich immer ab
            if (AktuelleRolle == RolleEnum.Alice)
            {
                AktuelleRolle = RolleEnum.Bob;
                return RolleEnum.Bob;
            }
            else
            {
                AktuelleRolle = RolleEnum.Alice;
                return RolleEnum.Alice;
            }
        }

        public void BerechneAktuellePhase(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            if (e.NewItems == null || e.NewItems!.Count != 1) return;
            Handlungsschritt neusterHandlungsschritt = (Handlungsschritt) e.NewItems[0]!;

            if (_aktuellePhase is 0 or 1 && neusterHandlungsschritt is {OperationsTyp: OperationsEnum.zugBeenden, Rolle: RolleEnum.Bob})
            {
                _aktuellePhase += 1;
                PropertyHasChanged(nameof(_aktuellePhase));
            }
            if (_aktuellePhase == 2 && neusterHandlungsschritt is { OperationsTyp: OperationsEnum.bitsStreichen, Rolle: RolleEnum.Alice })
            {
                _aktuellePhase += 1;
                PropertyHasChanged(nameof(_aktuellePhase));
            }
            if (_aktuellePhase == 3 && neusterHandlungsschritt is { OperationsTyp: OperationsEnum.bitfolgenVergleichen, Rolle: RolleEnum.Alice })
            {
                _aktuellePhase += 1;
                PropertyHasChanged(nameof(_aktuellePhase));
            }
            if (_aktuellePhase == 4 && neusterHandlungsschritt is { OperationsTyp: OperationsEnum.textEntschluesseln, Rolle: RolleEnum.Bob })
            {
                _aktuellePhase += 1;
                PropertyHasChanged(nameof(_aktuellePhase));
            }
        }

        public List<OperationsEnum> GebeHilfestellung(SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            return schwierigkeitsgrad switch
            {
                SchwierigkeitsgradEnum.Leicht => GebeHilfestellungLeicht(),
                SchwierigkeitsgradEnum.Mittel => GebeHilfestellungMittel(),
                SchwierigkeitsgradEnum.Schwer => new List<OperationsEnum>(), //Bei Schwer gibt es keine Hilfe
                _ => new List<OperationsEnum>(),
            };
        }

        private List<OperationsEnum> GebeHilfestellungLeicht()
        {
            //Die Leichte Hilfestellung gibt eine Liste zurück, in der nur die Operationen sind
            //welche in der aktuellen Phase und von der aktuellen Rolle erlaubt sein sollen

            //Informationen umbenennen ist immer erlaubt
            var op = new List<OperationsEnum> { OperationsEnum.informationUmbenennen };

            switch (_aktuellePhase)
            {
                case 0:
                {
                    if (AktuelleRolle == RolleEnum.Alice)
                    {
                        op.Add(OperationsEnum.textLaengeBestimmen);
                        op.Add(OperationsEnum.textGenerieren);
                        op.Add(OperationsEnum.zahlGenerieren);
                    }

                    break;
                }
                case 1 when AktuelleRolle == RolleEnum.Alice:
                    op.Add(OperationsEnum.bitfolgeGenerierenAngabe);
                    op.Add(OperationsEnum.bitfolgeGenerierenZahl);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenGenerieren);
                    break;
                case 1:
                    op.Add(OperationsEnum.polarisationsschemataGenerierenAngabe);
                    op.Add(OperationsEnum.polarisationsschemataGenerierenZahl);
                    op.Add(OperationsEnum.photonenZuBitfolge);
                    break;
                case 2 when AktuelleRolle == RolleEnum.Alice:
                    op.Add(OperationsEnum.bitsStreichen);
                    break;
                case 2:
                    op.Add(OperationsEnum.polschataVergleichen);
                    op.Add(OperationsEnum.bitsStreichen);
                    break;
                case 3 when AktuelleRolle == RolleEnum.Alice:
                    op.Add(OperationsEnum.zahlGenerieren);
                    op.Add(OperationsEnum.bitmaskeGenerieren);
                    op.Add(OperationsEnum.bitsStreichen);
                    op.Add(OperationsEnum.bitfolgenVergleichen);
                    op.Add(OperationsEnum.textLaengeBestimmen);
                    op.Add(OperationsEnum.bitfolgeNegieren);
                    break;
                case 3:
                    op.Add(OperationsEnum.bitsStreichen);
                    op.Add(OperationsEnum.bitfolgeNegieren);
                    break;
                case 4 when AktuelleRolle == RolleEnum.Alice:
                    op.Add(OperationsEnum.textVerschluesseln);
                    op.Add(OperationsEnum.bitfolgenVergleichen);
                    break;
                case 4:
                    op.Add(OperationsEnum.textEntschluesseln);
                    break;
            }

            return op;
        }

        private List<OperationsEnum> GebeHilfestellungMittel()
        {
            //Für die mittlere Schwierigkeit wird eine Liste zurückgegeben, die alle Operationen beinhaltet
            //die in der aktuellen Phase hilfreich sein können.

            //Weil in den Phasen mehr Operationen genutzt werden, werden hier die Operationen entfernt, die nicht benötigt werden.
            //Dazu wird die Liste erst mit allen möglichen Operationen gefüllt
            List<OperationsEnum> op = Enum.GetValues(typeof(OperationsEnum)).Cast<OperationsEnum>().ToList();

            switch (_aktuellePhase)
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
