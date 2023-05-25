using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using quaKrypto.Commands;

namespace quaKrypto.ViewModels
{
    public class HauptMenuViewModel : BaseViewModel
    {
        public DelegateCommand LobbyBeitritt { get; set; }

        public HauptMenuViewModel(Navigator navigator)
        {
            
        LobbyBeitritt = new((o) =>
            {
                navigator.aktuellesViewModel = new LobbyBeitrittViewModel(navigator);
                
            }, null);
        }

    }
}
