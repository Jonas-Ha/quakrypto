using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System.Collections.ObjectModel;
using quaKrypto.Models.Interfaces;
using System.Linq;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }

        private UebungsszenarioNetzwerkBeitrittInfo? uebungsszenarioNetzwerkBeitrittInfo = null;
        public UebungsszenarioNetzwerkBeitrittInfo? SelectedLobby { get {return uebungsszenarioNetzwerkBeitrittInfo;} set { uebungsszenarioNetzwerkBeitrittInfo = value; EigenschaftWurdeGeändert(); LobbyBeitreten.RaiseCanExecuteChanged(); } }

        //Gettet die Verfuegbaren Netzwerklobbys und reicht sie an das DataGrid durch
        public ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbarLobbys { get { return NetzwerkClient.VerfuegbareLobbys; } }

        public LobbyBeitrittViewModel(Navigator navigator)
        {
           
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
                if (SelectedLobby == null) return;
                //Informationserhalt der Ausgewählten Lobby --> Eigenes Übungsszenario erstellen im Nachfolgenden
                UebungsszenarioNetzwerkBeitrittInfo uebungsszenarioInfo = NetzwerkClient.VerfuegbareLobbys.Where(v => v.IPAddress.Equals(SelectedLobby.IPAddress)).First();
                IVariante variante = uebungsszenarioInfo.Variante switch
                {
                    "Normaler Ablauf" => new VarianteNormalerAblauf(uebungsszenarioInfo.StartPhase),
                    "Lauschangriff" => new VarianteAbhören(uebungsszenarioInfo.StartPhase),//TODO: Start und endphase in Info hinzufügen
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
