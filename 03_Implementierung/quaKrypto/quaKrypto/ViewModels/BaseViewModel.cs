using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Funktion aller ViewModels zur Benachrichtigung der Oberfläche das sich ein Wert geändert hat
        /// </summary>
        /// <param name="eigenschaftsName"></param>
        protected virtual void EigenschaftWurdeGeändert([CallerMemberName] string? eigenschaftsName = null)
        {
            //Invoken des PropertyCheanged Events
            if (string.IsNullOrEmpty(eigenschaftsName)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(eigenschaftsName));
        }
    }
}
