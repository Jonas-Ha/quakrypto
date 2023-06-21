using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using quaKrypto.Commands;
using quaKrypto.Models.Interfaces;
using quaKrypto.Models.Enums;

namespace quaKrypto.ViewModels
{
    public class HauptMenuViewModel : BaseViewModel
    {
        public DelegateCommand LobbyBeitritt { get; set; }
        public DelegateCommand LobbyErstellen { get; set; }

        public HauptMenuViewModel(Navigator navigator)
        {
            
        LobbyBeitritt = new((o) =>
            {
                navigator.aktuellesViewModel = new LobbyBeitrittViewModel(navigator);
                
            }, null);
        LobbyErstellen = new((o) =>
            {
                navigator.aktuellesViewModel = new LobbyErstellenViewModel(navigator);

                /*
                 * ZUM TESTEN FÜR DIE AUFZEICHNUNG
                 *
                RolleEnum rolleEnum = RolleEnum.Alice;
                string alias = "alias_alice";
                string passwort = "passwort_alice";
                RolleEnum rolleEnumEve = RolleEnum.Eve;
                string aliasEve = "alias_eve";
                string passwortEve = "passwort_eve";
                RolleEnum rolleEnumBob = RolleEnum.Bob;
                string aliasBob = "alias_bob";
                string passwortBob = "passwort_bob";
                RolleEnum rolleEnumAlice2 = RolleEnum.Alice;
                string aliasAlice2 = "alias_alice2";
                string passwortAlice2 = "passwort_alice2";


                string nameueb = "Ueb";

                //Arrange
                IVariante varianteNorm = new VarianteNormalerAblauf(1);
                UebungsszenarioLokal Ueb = new UebungsszenarioLokal(SchwierigkeitsgradEnum.Leicht, varianteNorm, 1, 4, nameueb);
                Rolle rolle = new Rolle(rolleEnum, alias, passwort);
                Rolle rolle2 = new Rolle(rolleEnumEve, aliasEve, passwortEve);
                Rolle rolle3 = new Rolle(rolleEnumBob, aliasBob, passwortBob);

                Ueb.RolleHinzufuegen(rolle);
                Ueb.RolleHinzufuegen(rolle3);

                Ueb.Starten();
                Ueb.AktuelleRolle.BeginneZug(passwort);

                // User macht Eingabe -> string text = "Hallo Bob!";
                Information inf0 = new Information(0, "int", InformationsEnum.zahl, 123, null);
                Information inf1 = new Information(0, "string", InformationsEnum.bitfolge, "Hallo Bob", null);

                Information erg0 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.zahlGenerieren, null, inf0, "eine Zahl", Ueb.AktuelleRolle.RolleTyp);
                Information erg1 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, inf1, "Hallo an Bob", Ueb.AktuelleRolle.RolleTyp);
                Information erg2 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textLaengeBestimmen, erg1, null, "Text Länge", Ueb.AktuelleRolle.RolleTyp);

                Ueb.NaechsterZug();
                Ueb.GebeBildschirmFrei(passwortBob);

                Information inf2 = new Information(0, "string", InformationsEnum.bitfolge, "Hallo Alice", null);

                Information erg3 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textGenerieren, null, inf2, "Hallo an Alice", Ueb.AktuelleRolle.RolleTyp);
                Information erg4 = Ueb.HandlungsschrittAusführenLassen(OperationsEnum.textLaengeBestimmen, erg3, null, "Text Länge", Ueb.AktuelleRolle.RolleTyp);

                navigator.aktuellesViewModel = new AufzeichnungViewModel(navigator, Ueb);
                */
            }, null);
        }

    }
}
