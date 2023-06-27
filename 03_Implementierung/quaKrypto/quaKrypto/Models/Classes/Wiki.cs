using System;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using quaKrypto.Services;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    public static class Wiki
    {
        private static readonly string WIKI_BENUTZERSEITEN_ORDNERNAME = Path.Combine(Environment.CurrentDirectory, "Wiki Benutzerseiten");
        private static ObservableCollection<WikiSeite> wikiSeiten = new() { new WikiSeite("Neue Seite", "") };
        public static ObservableCollection<WikiSeite> WikiSeiten => wikiSeiten;

        private static SchwierigkeitsgradEnum schwierigkeitsgrad = SchwierigkeitsgradEnum.Leicht;
        public static SchwierigkeitsgradEnum Schwierigkeitsgrad
        {
            get => schwierigkeitsgrad; set
            {
                if (schwierigkeitsgrad != value)
                {
                    schwierigkeitsgrad = value;
                    SpeichereBenutzerWikiSeiten();
                    WikiStandardseitenService.LadeAlleWikiSeitenMitSchwierigkeit(schwierigkeitsgrad);
                    LadeBenutzerWikiSeiten();
                }
            }
        }

        private static int indexDerSelektiertenSeite = 0;
        private static int IndexDerSelektiertenSeite
        {
            get => indexDerSelektiertenSeite;
            set
            {
                wikiSeiten.ElementAt(indexDerSelektiertenSeite < wikiSeiten.Count ? indexDerSelektiertenSeite : 0).SetzeAktivStatus(false);
                indexDerSelektiertenSeite = value;
                wikiSeiten.ElementAt(indexDerSelektiertenSeite).SetzeAktivStatus(true);
            }
        }
        public static WikiSeite SelektierteWikiSeite => wikiSeiten.ElementAt(indexDerSelektiertenSeite);

        private static bool wikiIstOffen = false;
        public static bool WikiIstOffen { get => wikiIstOffen; set => wikiIstOffen = value; }

        static Wiki()
        {
            if (!Directory.Exists(WIKI_BENUTZERSEITEN_ORDNERNAME)) _ = Directory.CreateDirectory(WIKI_BENUTZERSEITEN_ORDNERNAME);
            WikiStandardseitenService.ErzeugeAlleStandardWikiseiten();
            WikiStandardseitenService.LadeAlleWikiSeitenMitSchwierigkeit(Schwierigkeitsgrad);
            LadeBenutzerWikiSeiten();
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite;
        }

        private static void LadeBenutzerWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_BENUTZERSEITEN_ORDNERNAME);
            foreach (string datei in dateien)
            {
                if (File.Exists(datei)) wikiSeiten.Add(new WikiSeite(Path.GetFileName(datei).Split(") ")[1], File.ReadAllText(datei)));
            }
            IndexDerSelektiertenSeite = 0;
        }

        public static void SpeichereBenutzerWikiSeiten()
        {
            string[] dateien = Directory.GetFiles(WIKI_BENUTZERSEITEN_ORDNERNAME);
            foreach (string datei in dateien)
            {
                File.Delete(datei);
            }
            foreach (WikiSeite wikiSeite in WikiSeiten)
            {
                if(wikiSeite.IdentifierInteger > 5)
                {
                    File.WriteAllText(Path.Combine(WIKI_BENUTZERSEITEN_ORDNERNAME, $"({wikiSeite.Identifier}) {EntferneVerbotenesVonDateiNamen(wikiSeite.WikiSeiteName)}"), wikiSeite.Inhalt);
                    wikiSeite.SetzeAktivStatus(false);
                    wikiSeite.SetzeEditierModus(false);
                }
            }
            indexDerSelektiertenSeite = 0;
        }
        private static int BekommeIndexVonIdentifier(string identifier) => WikiSeiten.IndexOf(WikiSeiten.Where(wikiSeite => wikiSeite.Identifier == identifier).First());


        public static void SeitenErweitern()
        {
            wikiSeiten.Add(new WikiSeite("Neue Seite", ""));
            IndexDerSelektiertenSeite = wikiSeiten.Count - 1;
        }

        public static void SeiteEntfernen()
        {
            if (IndexDerSelektiertenSeite == 0 && wikiSeiten.Count <= 1) return;
            WikiSeiten.RemoveAt(IndexDerSelektiertenSeite);
            IndexDerSelektiertenSeite = IndexDerSelektiertenSeite >= WikiSeiten.Count ? --IndexDerSelektiertenSeite : IndexDerSelektiertenSeite;
        }

        public static void SeiteSelektieren(string identifier) => IndexDerSelektiertenSeite = BekommeIndexVonIdentifier(identifier);

        public static void SelektiereDieErsteSeite() => IndexDerSelektiertenSeite = 0;

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
