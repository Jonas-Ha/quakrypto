using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class AufzeichnungViewModel : BaseViewModel
    {
        private IUebungsszenario uebungsszenario;

        public ObservableCollection<Handlungsschritt> Handlungsschritte 
        {
            get { return uebungsszenario.Aufzeichnung.Handlungsschritte; }
        }

        public DelegateCommand HauptMenu { get; set; }

        public AufzeichnungViewModel(Navigator navigator, IUebungsszenario uebungsszenario)
        {
            Wiki.Schwierigkeitsgrad = Models.Enums.SchwierigkeitsgradEnum.Leicht;
            this.uebungsszenario = uebungsszenario;
            HauptMenu = new((o) =>
            {
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);

            }, null);
        }
    }
}
