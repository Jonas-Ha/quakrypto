
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Commands;
using quaKrypto.Models;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.ViewModels
{
    public class LobbyScreenViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;

        public DelegateCommand HauptMenu { get; set; }
        public LobbyScreenViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
        }
    }
}
