using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace quaKrypto.Commands
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<object> execute;
        private readonly Predicate<object>? canExecute;

        public DelegateCommand(Action<object> execute, Predicate<object>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke(parameter ?? new object()) ?? true;
        }

        public void Execute(object? parameter)
        {
            execute?.Invoke(parameter ?? new object());
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
