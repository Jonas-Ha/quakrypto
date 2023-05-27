using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        private DispatcherTimer timer;
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }
        public ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> VerfuegbarLobbys { get {return _verfuegbarLobbys; } set { _verfuegbarLobbys = value; } }
        private VarianteAbhoeren va = new VarianteAbhoeren(1);
        public LobbyBeitrittViewModel(Navigator navigator)
        {
            var aliceIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Alice/Alice_128px.png"));
            var bobIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Bob/Bob_128px.png"));
            var eveIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/Spiel/Eve/Eve_128px.png"));
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            
            HauptMenu = new((o) =>
            {
                timer.Stop();
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);
            }, null);

            LobbyBeitreten = new((o) =>
            {
                timer.Stop();
                navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, null);

            }, (o) => _ausgewaehlteLobby != -1);

            //LobbyBeitreten = new DelegateCommand((o) => { navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, null); }, (o) => _selectedIndex != -1); //canExcute funktioniert noch nicht
            //BeispielDaten
            _verfuegbarLobbys.Add(new UebungsszenarioNetzwerkBeitrittInfo { Lobbyname = "GreinerTraumwelt",Protokoll="BB84" ,Schwierigkeitsgrad = SchwierigkeitsgradEnum.leicht.ToString(), Variante = va.VariantenName, AliceIcon = aliceIcon, BobIcon = bobIcon, EveIcon = eveIcon});
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //hier zyklischer erhalt der Daten der Netzwerklobbys
        }

        private int _ausgewaehlteLobby = -1;
        private ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> _verfuegbarLobbys = new ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo>();
        public int AusgewaehlteLobby { get { return _ausgewaehlteLobby; } set { _ausgewaehlteLobby = value; this.EigenschaftWurdeGeändert(); this.LobbyBeitreten.RaiseCanExecuteChanged(); } }
        

    }
}
