using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }
        public ObservableCollection<UebungsszenarioNetzwerkBeitrittInfo> LobbyInformation { get; set; }
        public LobbyBeitrittViewModel(Navigator navigator)
        {

            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);

            LobbyBeitreten = new((o) =>
            {
                navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, null);

            }, (o) => _selectedIndex != -1);
            //LobbyBeitreten = new DelegateCommand((o) => { navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, null); }, (o) => _selectedIndex != -1); //canExcute funktioniert noch nicht
            }
        private int _selectedIndex = -1;
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; this.EigenschaftWurdeGeändert(); this.LobbyBeitreten.RaiseCanExecuteChanged(); } }
        

    }
}
