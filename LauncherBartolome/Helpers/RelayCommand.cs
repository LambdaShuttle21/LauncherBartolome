using System;
using System.Windows.Input;

namespace LauncherBartolome.Helpers
{
    public class RelayCommand : ICommand//Un command facilita la implementacion
                                        //de una accion o evento pero desacoplado del boton, eso permite al Command
                                        //ser reutilizado en diferentes partes del codigo, por ejemplo, en el caso de MVVM,
                                        //el Command se puede usar en la ViewModel y luego enlazarlo a un boton en la vista,
                                        //sin necesidad de escribir codigo especifico para cada boton. Eso es lo que se hizo
                                        //en el InicioView.xaml.cs, se creo un comando OpenAppCommand y se enlazo a un boton en la vista
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

