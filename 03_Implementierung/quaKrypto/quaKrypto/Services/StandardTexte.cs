using System;
using System.Collections.Generic;

namespace quaKrypto.Services
{
    public static class StandardTexte
    {
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
            "Summm"
        };

        public static string BekommeZufälligenText()
        {
            Random random = new(DateTime.Now.Millisecond);
            return texte[random.Next(texte.Count)];
        }

    }
}
