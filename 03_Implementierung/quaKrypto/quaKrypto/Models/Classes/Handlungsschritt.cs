// **********************************************************
// File: Handlungsschritt.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Handlungsschritt
    {
        private OperationsEnum operationsTyp;
        private Information operand1;
        private object operand2;
        private String ergebnisName;
        private Information ergebnis;
        private RolleEnum rolle;
        private uint aktuellePhase;
        private static Operationen op = new Operationen();
        private static Dictionary<OperationsEnum, Delegate> HandlungsschrittKommando =
            new Dictionary<OperationsEnum, Delegate>()
            {
                { OperationsEnum.nachrichtSenden, op.NachrichtSenden },
                { OperationsEnum.nachrichtEmpfangen, op.NachrichtEmpfangen },
                { OperationsEnum.nachrichtAbhoeren, op.NachrichtAbhoeren },
                { OperationsEnum.bitfolgeGenerierenZahl, op.BitfolgeGenerierenZahl },
                { OperationsEnum.bitfolgeGenerierenAngabe, op.BitfolgeGenerierenAngabe },
                { OperationsEnum.polarisationsschemataGenerierenZahl, op.PolarisationsschemataGenerierenZahl },
                { OperationsEnum.polarisationsschemataGenerierenAngabe, op.PolarisationsschemataGenerierenAngabe },
                { OperationsEnum.photonenGenerieren, op.PhotonenGenerieren },
                { OperationsEnum.bitmaskeGenerieren, op.BitmaskeGenerieren },
                { OperationsEnum.polschataVergleichen, op.PolschataVergleichen },
                { OperationsEnum.bitfolgenVergleichen, op.BitfolgenVergleichen },
                { OperationsEnum.bitfolgeNegieren, op.BitfolgeNegieren },
                { OperationsEnum.photonenZuBitfolge, op.PhotonenZuBitfolge },
                { OperationsEnum.textGenerieren, op.TextGenerieren },
                { OperationsEnum.textLaengeBestimmen, op.TextLaengeBestimmen },
                { OperationsEnum.textVerschluesseln, op.TextVerschluesseln },
                { OperationsEnum.textEntschluesseln, op.TextEntschluesseln },
                { OperationsEnum.bitsStreichen, op.BitsStreichen },
                { OperationsEnum.bitsFreiBearbeiten, op.BitsFreiBearbeiten },
                { OperationsEnum.zahlGenerieren, op.ZahlGenerieren },
                { OperationsEnum.zugBeenden, op.ZugBeenden },
            };

        public Handlungsschritt(uint informationsID, Enums.OperationsEnum operationsTyp, Information operand1, object operand2, String ergebnisName, RolleEnum rolle)
        {
            this.OperationsTyp = operationsTyp;
            this.Operand1 = operand1;
            this.Operand2 = operand2;
            this.ErgebnisName = ergebnisName;
            this.Rolle = rolle;

            Delegate del = null;

            if (HandlungsschrittKommando.TryGetValue(OperationsTyp, out del))
            {
                var Ergebnis = del.DynamicInvoke(informationsID, Operand1, Operand2, ErgebnisName) as Information;
                if ((Ergebnis != null)) this.Ergebnis = Ergebnis;
            }
            else throw new InvalidOperationException("Für diesen Operationstypen wurde keine Operation gefunden");
        }

        public OperationsEnum OperationsTyp
        {
            get { return operationsTyp; }
            init { operationsTyp = value; }
        }

        public Information Operand1
        {
            get { return operand1; }
            init { operand1 = value; }
        }

        public object Operand2
        {
            get { return operand2; }
            init { operand2 = value; }
        }

        public String ErgebnisName
        {
            get { return ergebnisName; }
            set { ergebnisName = value; }
        }

        public Information Ergebnis
        {
            get { return ergebnis; }
            set { ergebnis = value; }
        }

        public RolleEnum Rolle
        {
            get { return rolle; }
            init { rolle = value; }
        }

        public uint AktuellePhase
        {
            get { return aktuellePhase; }
            set { aktuellePhase = value; }
        }
    }
}
