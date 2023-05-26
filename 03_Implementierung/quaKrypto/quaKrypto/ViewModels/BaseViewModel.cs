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


        protected virtual void EigenschaftWurdeGeändert([CallerMemberName] string? eigenschaftsName = null)
        {
            if (string.IsNullOrEmpty(eigenschaftsName)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(eigenschaftsName));
        }
    }
}
