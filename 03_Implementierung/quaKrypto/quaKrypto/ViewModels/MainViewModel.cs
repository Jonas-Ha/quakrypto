using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly Navigator _navigator;
        //Setzen des aktuellen ViewModels(Property)
        public BaseViewModel AktuellesViewModel => _navigator.aktuellesViewModel;

        public MainViewModel(Navigator navigator)
        {
            _navigator = navigator;
            //Hinzufügen als Event
            navigator.aktuellesViewModelGeaendert += aktuellesViewModelGeandert;
        }
        //Aktuelles View Model wurde geändert
        private void aktuellesViewModelGeandert()
        {
            EigenschaftWurdeGeändert(nameof(AktuellesViewModel));
        }
    }
}
