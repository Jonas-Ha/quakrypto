using quaKrypto.Commands;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class RolleEntsperrenViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;
        //Zurück zum Hauptmenü Command
        public DelegateCommand HauptMenu { get; set; }
        public RolleEntsperrenViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
        }
    }
}
