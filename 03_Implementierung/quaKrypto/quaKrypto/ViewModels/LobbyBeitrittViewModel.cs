using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        //private DispatcherTimer timer;
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }
        //default value: keine Lobby im DataGrid ausgewählt
        private int _ausgewaehlteLobby = -1;
        //Property welches den SelectedIndex des Datagrids erhält
        public int AusgewaehlteLobby { get { return _ausgewaehlteLobby; } set { _ausgewaehlteLobby = value; this.EigenschaftWurdeGeändert(); this.LobbyBeitreten.RaiseCanExecuteChanged(); } }
        //Gettet die Verfuegbaren Netzwerklobbys und reicht sie an das DataGrid durch
        public ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbarLobbys { get { return NetzwerkClient.VerfuegbareLobbys; }}
        
        public LobbyBeitrittViewModel(Navigator navigator)
        {
            /*
            var aliceIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Alice/Alice_128px.png"));
            var bobIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Bob/Bob_128px.png"));
            var eveIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Eve/Eve_128px.png"));
            */
            //Hier wird der Anstoß gegeben nach verfübaren Lobbys zu suchen
            NetzwerkClient.BeginneSucheNachLobbys();
            HauptMenu = new((o) =>
            {
                //Hier wird die suche beendet und dann zum Hauptmenü zurück navigiert
                NetzwerkClient.BeendeSucheNachLobbys();
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);

            LobbyBeitreten = new((o) =>
            {
                //Hier wird sich mit dem ausgwählten Übungsszeanrio verbunden, die Suche beendet und dann weiter zum Lobbyscreen gegangen
                UebungsszenarioNetzwerkBeitrittInfo uebungsszenarioInfo =
                    NetzwerkClient.VerfuegbareLobbys[AusgewaehlteLobby];
                NetzwerkClient.VerbindeMitUebungsszenario(uebungsszenarioInfo);
                NetzwerkClient.BeendeSucheNachLobbys();
                IVariante variante = uebungsszenarioInfo.Variante switch
                {
                    "Normaler Ablauf" => new VarianteNormalerAblauf(uebungsszenarioInfo.StartPhase),
                    "Lauschangriff" => new VarianteAbhoeren(uebungsszenarioInfo.StartPhase),//TODO: Start und endphase in Info hinzufügen
                    "Man-In-The-Middle" => new VarianteManInTheMiddle(uebungsszenarioInfo.StartPhase),
                    _ => new VarianteNormalerAblauf(uebungsszenarioInfo.StartPhase),
                };
                IUebungsszenario uebungsszenario = new UebungsszenarioNetzwerk(uebungsszenarioInfo.Schwierigkeitsgrad,
                    variante, uebungsszenarioInfo.StartPhase, uebungsszenarioInfo.EndPhase,
                    uebungsszenarioInfo.Lobbyname, false);
                navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenario);
                

            }, (o) => _ausgewaehlteLobby != -1);
        }

    }
}
