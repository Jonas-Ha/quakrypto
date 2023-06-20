// **********************************************************
// File: Rolle.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.Models.Classes
{
    [Serializable]
    public class Rolle
    {
        private RolleEnum rolle;
        private String alias;
        private String passwort;
        private bool freigeschaltet;
        private ObservableCollection<Information> informationsablage;
        public  ReadOnlyObservableCollection<Information> Informationsablage;
        private int informationszaehler;
        public List<Handlungsschritt> handlungsschritte; //Eigentlich nur für das Netzwerk benötigt

        //Dieser Konstruktor ist nur für die View zum Anzeigen in der LobbyView
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

        public String Alias 
        { 
            get { return alias; }
            init { alias = value; }
        }

        public Enums.RolleEnum RolleTyp
        {
            get { return rolle; }
        }

        public int InformationsZaehler
        {
            get { return informationszaehler; }
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
            if(freigeschaltet)
            {
                var handlungsschritt = new Handlungsschritt(informationszaehler++, operationsTyp, operand1, operand2, ergebnisInformationsName, rolle);
                if (operationsTyp == OperationsEnum.zugBeenden) freigeschaltet = false;
                Add(handlungsschritt);
                //handlungsschritte.Add(handlungsschritt);
                return handlungsschritt;

            }
            throw new Exception("Rolle war nicht freigeschaltet");
           
        }

        public void SpeicherInformationAb(Information information)
        {
            if (freigeschaltet)
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
