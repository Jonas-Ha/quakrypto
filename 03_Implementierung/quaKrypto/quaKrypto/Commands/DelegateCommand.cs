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
        //EventHandler und execute und canExecute objekte
        public event EventHandler? CanExecuteChanged;
        private readonly Action<object> execute;
        private readonly Predicate<object>? canExecute;
        //Konstruktor zum hinzufügen was ausgeführt werden soll, und der Bedingung durch die executed werden darf
        public DelegateCommand(Action<object> execute, Predicate<object>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        //Hier ohne CanExecute, kann immer alles ausgeführt werden
        public DelegateCommand(Action<object> execute) : this(execute, null) { }
        //CanExecute Property
        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke(parameter ?? new object()) ?? true;
        }
        //Execute Property
        public void Execute(object? parameter)
        {
            execute?.Invoke(parameter ?? new object());
        }
        //Funktion zum Überprüfen ob sich die CanExecute Eigenschaft geändert hat
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
