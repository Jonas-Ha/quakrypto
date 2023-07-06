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
        /// <summary>
        /// Die Liste der durchgeführten Handlungsschritte
        /// Wird aus dem uebungsszenario bezogen
        /// </summary>
        public ObservableCollection<Handlungsschritt> Handlungsschritte 
        {
            get { return uebungsszenario.Aufzeichnung.Handlungsschritte; }
        }
        /// <summary>
        /// Command zur Rückkehr ins Hauptmenü
        /// </summary>
        public DelegateCommand HauptMenu { get; set; }
        /// <summary>
        /// Konstruktor des ViewModels
        /// </summary>
        /// <param name="navigator"></param>
        /// <param name="uebungsszenario"></param>
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
