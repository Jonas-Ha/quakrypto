// **********************************************************
// File: Rolle.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.Models.Classes
{
    public class Rolle
    {
        private Enums.RolleEnum rolle;
        private String alias;
        private String passwort;
        private List<Information> informationsablage;
        private uint informationszaehler;
        public ObservableCollection<Handlungsschritt> handlungsschritte;
        public event EventHandler handlungsschrittVerfuegbar;

        //Dieser Konstruktor ist nur für die View zum Anzeigen in der LobbyView
        public Rolle(RolleEnum rolle, string alias)
        {
            this.rolle = rolle;
            this.alias = alias;
        }

        public Rolle(RolleEnum rolle, string alias, string passwort, IVariante variante)
        {
            this.informationszaehler = 0;
            this.rolle = rolle;
            this.alias = alias;
            this.passwort = passwort;
            informationsablage = new List<Information>();
            handlungsschritte = new ObservableCollection<Handlungsschritt>();
            handlungsschritte.CollectionChanged += variante.BerechneAktuellePhase;
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
        
        public bool BeginneZug(string passwort)
        {
            if (this.passwort == passwort)return true;
            else return false;
        }
        
        public Handlungsschritt ErzeugeHandlungsschritt(Enums.OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
           return new Handlungsschritt(informationszaehler++, operationsTyp, operand1, operand2, ergebnisInformationsName, rolle);
        }

        public void SpeicherInformationAb(Information information)
        {
            informationsablage.Add(information);   
        }

        public void LoescheInformation(Information information)
        {
            informationsablage.Remove(information);
        }

        public void AktualisiereInformationsZaehler(uint informationszaehler)
        {
            this.informationszaehler = informationszaehler;
        }
    }
}
