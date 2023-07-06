// **********************************************************
// File: Handlungsschritt.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Handlungsschritt
    {
        // gibt an, welche Operation in dem Handlungsschritt ausgeführt werden soll
        private OperationsEnum operationsTyp;

        private Information operand1;
        private object operand2;

        // Ergebnis wird im Konstruktor dieser Klasse berechnet durch den Aufruf der konkreten Operation
        private string ergebnisName;
        private Information ergebnis;

        // Rolle, die den ausgeführten Handlungsschritt empfangen soll (nur in <NachrichtSenden> relevant!)
        private RolleEnum rolle;

        private uint aktuellePhase;
        // Erzeugung eines Objekts des Typs <Operationen>, welches die eigentliche Operation in diesem Handlungsschritt ausführt
        // Zu einem Handlungsschritt gehört somit immer eine Operation!
        private static Operationen op = new Operationen();

        // Mapping der Enum-Werte des Enums <OperationsEnum> auf die implementierten Methoden der Klasse Handlungsschritt
        // erlaubt einfache Anpassung durch { OperationsEnum.xxx, op.xxx }
        // Methode wird in der Klasse <Operationen> implementiert
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
                { OperationsEnum.informationUmbenennen, op.InformationUmbenennen },
                { OperationsEnum.zugBeenden, op.ZugBeenden },
            };

        public Handlungsschritt() { }

        public Handlungsschritt(int informationsID, Enums.OperationsEnum operationsTyp, Information operand1, object operand2, String ergebnisName, RolleEnum rolle)
        {
            this.OperationsTyp = operationsTyp;
            this.Operand1 = operand1;
            this.Operand2 = operand2;
            this.ErgebnisName = ergebnisName;
            this.Rolle = rolle;

            Delegate del = null;

            if (HandlungsschrittKommando.TryGetValue(OperationsTyp, out del))
            {
                // Rolle wird nur in der Operation <NachrichtSenden> benötigt; gibt die Rolle des Empfängers an
                // --> Berechnung des Ergebnisses des Handlungsschritts durch Aufruf der gemappten Methode der Klasse <Operationen>
                var Ergebnis = del.DynamicInvoke(informationsID, Operand1, Operand2, ErgebnisName, Rolle) as Information;
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

        public string ErgebnisName
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
