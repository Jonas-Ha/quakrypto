using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class AufzeichnungViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;

        public List<Handlungsschritt> Handlungsschritte 
        {
            get { return uebungsszenario.Aufzeichnung.Handlungsschritte; }
        }

        public DelegateCommand HauptMenu { get; set; }

        public AufzeichnungViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
        }
    }
}
