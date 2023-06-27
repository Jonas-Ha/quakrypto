using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System.Collections.ObjectModel;
using quaKrypto.Models.Interfaces;
using System.Linq;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        //private DispatcherTimer timer;
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }

        private UebungsszenarioNetzwerkBeitrittInfo? uebungsszenarioNetzwerkBeitrittInfo = null;
        public UebungsszenarioNetzwerkBeitrittInfo? SelectedLobby { get {return uebungsszenarioNetzwerkBeitrittInfo;} set { uebungsszenarioNetzwerkBeitrittInfo = value; EigenschaftWurdeGeändert(); LobbyBeitreten.RaiseCanExecuteChanged(); } }

        //Gettet die Verfuegbaren Netzwerklobbys und reicht sie an das DataGrid durch
        public ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbarLobbys { get { return NetzwerkClient.VerfuegbareLobbys; } }

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
                //UebungsszenarioNetzwerkBeitrittInfo uebungsszenarioInfo = NetzwerkClient.VerfuegbareLobbys[AusgewaehlteLobby];
                if (SelectedLobby == null) return;
                UebungsszenarioNetzwerkBeitrittInfo uebungsszenarioInfo = NetzwerkClient.VerfuegbareLobbys.Where(v => v.IPAddress.Equals(SelectedLobby.IPAddress)).First();
                IVariante variante = uebungsszenarioInfo.Variante switch
                {
                    "Normaler Ablauf" => new VarianteNormalerAblauf(uebungsszenarioInfo.StartPhase),
                    "Lauschangriff" => new VarianteAbhoeren(uebungsszenarioInfo.StartPhase),//TODO: Start und endphase in Info hinzufügen
                    "Man-In-The-Middle" => new VarianteManInTheMiddle(uebungsszenarioInfo.StartPhase),
                    _ => new VarianteNormalerAblauf(uebungsszenarioInfo.StartPhase),
                };
                IUebungsszenario uebungsszenario = new UebungsszenarioNetzwerk(uebungsszenarioInfo.Schwierigkeitsgrad, variante, uebungsszenarioInfo.StartPhase, uebungsszenarioInfo.EndPhase, uebungsszenarioInfo.Lobbyname, false);

                NetzwerkClient.Ubungsszenario = (UebungsszenarioNetzwerk)uebungsszenario;
                NetzwerkClient.BeendeSucheNachLobbys();
                NetzwerkClient.VerbindeMitUebungsszenario(uebungsszenarioInfo);


                navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenario, false);


            }, (o) => SelectedLobby != null);
        }

    }
}
