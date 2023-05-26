using quaKrypto.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class LobbyBeitrittViewModel : BaseViewModel
    {
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyBeitreten { get; set; }

        public LobbyBeitrittViewModel(Navigator navigator)
        {

            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);

            LobbyBeitreten = new((o) =>
            {
                //navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator);

            }, null);
        }
    }
}
