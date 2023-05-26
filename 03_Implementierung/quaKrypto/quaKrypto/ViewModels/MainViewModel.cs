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
        public BaseViewModel AktuellesViewModel => _navigator.aktuellesViewModel;

        public MainViewModel(Navigator navigator)
        {
            _navigator = navigator;

            navigator.aktuellesViewModelGeaendert += aktuellesViewModelGeandert;
        }

        private void aktuellesViewModelGeandert()
        {
            EigenschaftWurdeGeändert(nameof(AktuellesViewModel));
        }
    }
}
