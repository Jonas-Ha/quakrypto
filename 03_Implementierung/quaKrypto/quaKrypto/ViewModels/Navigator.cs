using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class Navigator
    {
        //event und Aktuelles View model
        public event Action? aktuellesViewModelGeaendert;

        private BaseViewModel? _aktuellesviewModel;

        //Property des aktuellen ViewModels
        public BaseViewModel aktuellesViewModel 
        {
            get => _aktuellesviewModel;
            set
            {
                _aktuellesviewModel = value;
                ViewModelWurdeGeändert();
            }
        }

        private void ViewModelWurdeGeändert()
        {
            aktuellesViewModelGeaendert?.Invoke();
        }
    }
}
