// **********************************************************
// File: VarianteNormalerAblauf.cs
// Autor: Erik Barthelmann, Jonas Hammer
// erstellt am: 20.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Interfaces
{
    public interface IUebungsszenario
    {
        public List<Rolle> Rollen
        { get; }

        public Enums.SchwierigkeitsgradEnum Schwierigkeitsgrad
        { get; }

        public IVariante Variante 
        { get; }

        public uint StartPhase
        { get; }

        public uint EndPhase
        { get; }

        public Uebertragungskanal Uebertragungskanal
        { get; }

        public Aufzeichnung Aufzeichnung
        { get; }

        public string Name
        { get; }

        //Diese Funktionen werden dem View Model als Interface angeboten

        // Füge ein Rollen Objekt zum Übungsszenario hinzu
        // Gibt false zurück, wenn der RollenTyp bereits belegt ist
        public bool RolleHinzufuegen(Rolle rolle);

        // Entferne ein Rolle mit bestimmten Typ
        public void GebeRolleFrei(RolleEnum rolle);

        //Wird aufgerufen wenn das Spiel gestartet wird.
        public void Starten();

        //Wird aufgerufen wenn ein Benutzer auf Zug Beenden klickt
        //Gibt false zurück wenn das Übungsszenario durchgespielt wurde
        public bool NaechsterZug();

        //Wird aufgerufen wenn ein Benutzer versucht den gesperrten Bildschirm zu entsperren
        //Gibt false zurück wenn das Passwort nicht mit der aktuellen Rolle übereinstimmt - true wenn es passt
        public bool GebeBildschirmFrei(String Passwort);

        //Gibt ein Handlungsschritt objekt and die aktuelle rolle weiter, die ihn dann ausführt
        //Gibt das dadurch erstellte Informationsobjekt zurück
        public Information HandlungsschrittAusführenLassen(Handlungsschritt handlungsschritt);

        //Speichert die Information mit der übergebenen ID im Speicher der aktuellen Rolle ab
        public void SpeichereInformationenAb(uint informationID);
        //Löscht die Information mit der übergebenen ID aus dem Speicher der aktuellen Rolle
        public void LoescheInformation(uint informationID);
        //Soll aufgerufen werden wenn das Übungsszenario beendet werden soll
        public void Beenden();

    }
}
