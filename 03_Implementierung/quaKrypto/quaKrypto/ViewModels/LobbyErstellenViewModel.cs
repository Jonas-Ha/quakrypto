using quaKrypto.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class LobbyErstellenViewModel : BaseViewModel
    {
        public DelegateCommand HauptMenu { get; set; }

        public DelegateCommand LobbyErstellen { get; set; }
        public LobbyErstellenViewModel(Navigator navigator)
        {

            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
            LobbyErstellen = new((o) =>
            {
                //navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator);

            }, null);
        }
    }
}
