// **********************************************************
// File: StandardTexte.cs
// Autor: Daniel Hannes
// erstellt am: 27.06.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;

namespace quaKrypto.Services
{
    //Diese Klasse ist nur dazu da, um dem Benutzer, wenn er eine StartPhase ungleich 0 ausgewählt hat, einen Geheimtext zu geben,
    //um daraufhin alles weitere von diesem Geheimtext aus zu berechnen.
    public static class StandardTexte
    {
        //Das ist die selbst erstellte Liste, welche die einzelnen Wörter in sich trägt.
        private static readonly List<string> texte = new()
        {
            "Multilingual",
            "Apfelkuchen",
            "Gerätewerk",
            "Feuerzeug",
            "Inbusschüssel",
            "Quantenkryptographie",
            "Softwareprojekt",
            "Hardwareprojekt",
            "Wasserflasche",
            "Amerika",
            "Afrika",
            "Europa",
            "Asien",
            "Antarktis",
            "Charles Bennett",
            "Gilles Brassard",
            "Alice Bob Eve",
            "Sonnenschein",
            "Kreativität",
            "Leonberg",
            "Amberg",
            "Plattlschützen",
            "Taubeninvasion",
            "Schornsteinfeger",
            "Hardfault",
            "iMendeffekt",
            "Nelson Mendela",
            "Mende View View Mende",
            "GrünerD",
            "Lionhill",
            "Mariahilfberg",
            "ViewModelMende",
            "MendeVP",
            "Atomkraft",
            "Apfelmus",
            "Stunden farmen",
            "mus",
            "Fehler???",
            "Wassermelone",
            "Hinteres Ende der Banane",
            "Himbeerpalast",
            "Eierlegende-Wollmilch-Frieda",
            "Honig-schleudererer",
            "Bienenkönigin",
            "Tot?",
            "Brotwolf",
            "Collisiondetection",
            "Fensterbrett",
            "Doppelgänger",
            "Betriebswirtschaftslehre",
            "Summm",
            "WochenMende",
            "MentellyDad"
        };

        //Das ist der Seed, welcher Benutzt wird, um zufällig ein Wort aus der Liste auszuwählen.
        //Der Seed wird beim Starten des Spiels übertragen, um für alle Benutzer die selben Wörter zu berechnen.
        private static int seed;
        public static int Seed { get => seed; set { seed = value; random = new Random(seed); } }
        private static Random random = new(Seed);

        //Das ist die Methode, um ein zufälliges Wort zu bekommen.
        public static string BekommeZufälligenText() => texte[random.Next(texte.Count)];

    }
}
