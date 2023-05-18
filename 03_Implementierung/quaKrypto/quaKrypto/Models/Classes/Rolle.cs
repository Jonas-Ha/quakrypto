// **********************************************************
// File: Rolle.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    internal class Rolle
    {
        private Enums.RolleEnum rolle;
        private string alias;
        private string passwort;
        private List<Information> informationsablage;
        private uint informationszaehler;
        private List<Handlungsschritt> zug;

        Rolle(Enums.RolleEnum rolle, string alias, string passwort)
        {
            this.rolle = rolle;
            this.alias = alias;
            this.passwort = passwort;
            informationsablage = new List<Information>();
        }

        bool BeginneZug(string passwort)
        {
            if (this.passwort == passwort)
            {
                zug = new List<Handlungsschritt>();

                return true;
            }
            else return false;
        }

        List<Handlungsschritt>? BeendeZug()
        {
            if (zug != null) return zug;
            else return null;
        }

        void ErzeugeHandlungsschritt(Enums.OperationsEnum operationsTyp, Information operand1, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
           // zug.Add(new Handlungsschritt(operationsTyp, operand1, ergebnisInformationsName, rolle));
        }

        void ErzeugeHandlungsschritt(Enums.OperationsEnum operationsTyp, Information operand1, Information operand2, String ergebnisInformationsName, Enums.RolleEnum rolle)
        {
           // zug.Add(new Handlungsschritt(operationsTyp, operand1, operand2, ergebnisInformationsName, rolle));
        }
    }
}
