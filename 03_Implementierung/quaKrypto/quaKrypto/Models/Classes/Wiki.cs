// **********************************************************
// File: Wiki.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using quaKrypto.Services;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    //Diese Klasse repräsentiert das Wiki mit der ganzen Logik, welche dahinter steckt.
    public static class Wiki
    {
        //Das sind die Propertys dieser Klasse.
        private static readonly string WIKI_BENUTZERSEITEN_ORDNERNAME = Path.Combine(Environment.CurrentDirectory, "Wiki Benutzerseiten");
        public static ObservableCollection<WikiSeite> WikiSeiten { get; } = new() { new WikiSeite("Neue Seite", "") };

        //Der Schwierigkeitsgrad gibt an, welche Seiten des Wikis geladen sein sollen und welche nicht.
        private static SchwierigkeitsgradEnum schwierigkeitsgrad = SchwierigkeitsgradEnum.Leicht;
        public static SchwierigkeitsgradEnum Schwierigkeitsgrad
        {
            get => schwierigkeitsgrad;
            set
            {
                //Wenn sich der Schwierigkeitsgrad verändert, so sollen andere Wikiseiten geladen werden.
                if (schwierigkeitsgrad == value) return;
                schwierigkeitsgrad = value;
                SpeichereBenutzerWikiSeiten();
                WikiStandardseitenService.LadeAlleWikiSeitenMitSchwierigkeit(schwierigkeitsgrad);
                LadeBenutzerWikiSeiten();
            }
        }

        //Der index der Selektierten Seite gibt an, welche WikiSeite gerade ausgewählt ist.
        private static int indexDerSelektiertenSeite = 0;
        private static int IndexDerSelektiertenSeite
        {
            get => indexDerSelektiertenSeite;
            set
            {
                //Wenn sich der Index der selektierten Seite ändert, so wird die vorher ausgewählte Wikiseite darüber informiert sowie die neu ausgewählte.
                WikiSeiten.ElementAt(indexDerSelektiertenSeite < WikiSeiten.Count ? indexDerSelektiertenSeite : 0).SetzeAktivStatus(false);
                indexDerSelektiertenSeite = value;
                WikiSeiten.ElementAt(indexDerSelektiertenSeite).SetzeAktivStatus(true);
            }
        }
        public static WikiSeite SelektierteWikiSeite => WikiSeiten.ElementAt(indexDerSelektiertenSeite);

        public static bool WikiIstOffen { get; set; }

        //Im Konstruktor wird der Ordner für die vom Benutzer angelegten WikiSeiten erstellt, falls noch nicht vorhanden.
        //Außerdem werden die Standardseiten geladen, sowie alle Seiten, welche vom Benutzer angelegt wurden.
        static Wiki()
        {
            if (!Directory.Exists(WIKI_BENUTZERSEITEN_ORDNERNAME)) _ = Directory.CreateDirectory(WIKI_BENUTZERSEITEN_ORDNERNAME);
            WikiStandardseitenService.ErzeugeAlleStandardWikiseiten();
            WikiStandardseitenService.LadeAlleWikiSeitenMitSchwierigkeit(Schwierigkeitsgrad);
            LadeBenutzerWikiSeiten();
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite;
        }

        //In dieser Methode werden alle Seiten geladen, welche der Benutzer angelegt hat.
        private static void LadeBenutzerWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_BENUTZERSEITEN_ORDNERNAME);
            foreach (string datei in dateien)
            {
                if (File.Exists(datei)) WikiSeiten.Add(new WikiSeite(Path.GetFileName(datei).Split(") ")[1], File.ReadAllText(datei)));
            }
            IndexDerSelektiertenSeite = 0;
        }

        //Hier werden alle Seiten abgespeichert, welche der Benutzer angelegt hat.
        public static void SpeichereBenutzerWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_BENUTZERSEITEN_ORDNERNAME);
            //Zuerst werden alle alten Dateien gelöscht
            foreach (string datei in dateien)
            {
                File.Delete(datei);
            }
            //Und dann neue Dateien mit den Wikiseiten erzeugt.
            foreach (WikiSeite wikiSeite in WikiSeiten)
            {
                if (wikiSeite.IdentifierInteger > 5)
                {
                    File.WriteAllText(Path.Combine(WIKI_BENUTZERSEITEN_ORDNERNAME, $"({wikiSeite.Identifier}) {EntferneVerbotenesVonDateiNamen(wikiSeite.WikiSeiteName)}"), wikiSeite.Inhalt);
                    wikiSeite.SetzeAktivStatus(false);
                    wikiSeite.SetzeEditierModus(false);
                }
            }
            indexDerSelektiertenSeite = 0;
        }

        //Dies ist eine Hilfsmethode, um anhand eines Identifiers einen Index zu bekommen.
        private static int BekommeIndexVonIdentifier(string identifier) => WikiSeiten.IndexOf(WikiSeiten.Where(wikiSeite => wikiSeite.Identifier == identifier).First());

        //Diese Methode wird vom ViewModel aufgerufen, wenn der Benutzer eine neue Seite anlegen will.
        public static void SeitenErweitern()
        {
            WikiSeiten.Add(new WikiSeite("Neue Seite", ""));
            IndexDerSelektiertenSeite = WikiSeiten.Count - 1;
        }

        //Diese Methode wird vom ViewModel aufgerufen, wenn der Benutzer eine alte Seite löschen will.
        public static void SeiteEntfernen()
        {
            if (IndexDerSelektiertenSeite == 0 && WikiSeiten.Count <= 1) return;
            WikiSeiten.RemoveAt(IndexDerSelektiertenSeite);
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite >= WikiSeiten.Count ? --IndexDerSelektiertenSeite : IndexDerSelektiertenSeite;
        }

        //Diese Methode wird vom ViewModel aufgerufen, wenn der Benutzer eine neue Seite auswählt.
        public static void SeiteSelektieren(string identifier) => IndexDerSelektiertenSeite = BekommeIndexVonIdentifier(identifier);

        //Hier wird die erste Seite des Wikis selektiert.
        public static void SelektiereDieErsteSeite() => IndexDerSelektiertenSeite = 0;

        //Dies ist eine Hilfsmethode, um aus den Namen von der Wikiseite alle verbotenen Zeichen für Windows zu entfernen. 
        private static string EntferneVerbotenesVonDateiNamen(string stringWelcherBereinigtWerdenSoll)
        {
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('<', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('>', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace(':', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('"', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('/', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('\\', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('|', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('?', ' ');
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace('*', ' ');

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("CON", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("PRN", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("AUX", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("NUL", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM1", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM2", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM3", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM4", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM5", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM6", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM7", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM8", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("COM9", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT1", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT2", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT3", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT4", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT5", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT6", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT7", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT8", " ");
            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace("LPT9", " ");

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Trim();
            if (stringWelcherBereinigtWerdenSoll[^1] == '.') stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll[..^1];

            stringWelcherBereinigtWerdenSoll = stringWelcherBereinigtWerdenSoll.Replace(") ", " ");
            return stringWelcherBereinigtWerdenSoll;
        }
    }
}
