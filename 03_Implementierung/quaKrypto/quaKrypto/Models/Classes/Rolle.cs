// **********************************************************
// File: Rolle.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Rolle
    {
        /*
         * Die möglichen Typen sind dem Enum <RolleEnum> zu entnehmen (alice, bob, eve)
         * Jede Rolle besitzt einen Alias und ein Passwort.
         */
        private RolleEnum rolle;
        private String alias;
        private String passwort;

        private bool freigeschaltet;
        private int informationszaehler;

        private ObservableCollection<Information> informationsablage;
        public ReadOnlyObservableCollection<Information> Informationsablage;

        // wird für das Übungsszenario im Netzwerk benötigt
        public List<Handlungsschritt> handlungsschritte;

        // Dieser Konstruktor ist nur für die View zum Anzeigen in der LobbyView
        public Rolle(RolleEnum rolle, string alias)
        {
            this.rolle = rolle;
            this.alias = alias;
        }

        public Rolle(RolleEnum rolle, string alias, string passwort)
        {
            this.informationszaehler = 0;
            this.rolle = rolle;
            this.alias = alias;
            this.passwort = passwort;
            this.freigeschaltet = false;
            informationsablage = new ObservableCollection<Information>();
            Informationsablage = new ReadOnlyObservableCollection<Information>(informationsablage);
            handlungsschritte = new List<Handlungsschritt>();
        }

        public string Alias
        {
            get { return alias; }
            init { alias = value; }
        }

        public RolleEnum RolleTyp
        {
            get { return rolle; }
        }

        public int InformationsZaehler
        {
            get { return informationszaehler; }
        }
        public bool Freigeschaltet
        {
            get { return freigeschaltet; }
        }

        public bool BeginneZug(string passwort)
        {
            if (this.passwort == passwort)
            {
                freigeschaltet = true;
                return true;
            }
            else return false;
        }

        public void Add(Handlungsschritt handlungsschritt)
        {
            handlungsschritte.Add(handlungsschritt);
        }

        public Handlungsschritt ErzeugeHandlungsschritt(Enums.OperationsEnum operationsTyp, Information operand1, object operand2, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
            if (freigeschaltet)
            {
                // Handlungsschritt ausführen
                var handlungsschritt = new Handlungsschritt(informationszaehler++, operationsTyp, operand1, operand2, ergebnisInformationsName, rolle);
                // nach Handlungsschritt ZugBeenden wird freigeschaltet auf 'false' gesetzt und an die Liste aus Handlungsschritten angehängt
                if (operationsTyp == OperationsEnum.zugBeenden) freigeschaltet = false;
                Add(handlungsschritt);
                // Liste aus Handlungsschritten wird zurückgegeben
                return handlungsschritt;
            }
            throw new Exception("Rolle war nicht freigeschaltet");
        }

        // legt eine Information in der Informationsablage einer Rolle ab
        public void SpeicherInformationAb(Information information, bool KI = false)
        {
            if (freigeschaltet || KI)
            {
                if (information == null) throw new NoNullAllowedException("Abzuspeichernde Information darf nicht null sein");
                for (int i = 0; i < informationsablage.Count; i++)
                {
                    if (informationsablage[i].InformationsID == information.InformationsID) return;
                }
                informationsablage.Add(information);
                return;
            }
            throw new Exception("Rolle war nicht freigeschaltet");
        }

        // entfernt eine Information aus der Informationsablage einer Rolle
        public bool LoescheInformation(int informationsID)
        {
            if (freigeschaltet)
            {
                for (int i = 0; i < informationsablage.Count; i++)
                {
                    if (informationsablage[i].InformationsID == informationsID)
                    {
                        informationsablage.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            throw new Exception("Rolle war nicht freigeschaltet");
        }

        public void AktualisiereInformationsZaehler(int informationszaehler)
        {
            this.informationszaehler = informationszaehler;
        }
    }
}
