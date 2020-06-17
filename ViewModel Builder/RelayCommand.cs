using System;
using System.Windows.Input;

namespace Frontend
{
    public class RelayCommand : ICommand
    {
        Action targetExecuteMethod;
        Func<bool> targetCanExecuteMethod;

        public RelayCommand(Action executeMethod)
        {
            targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            targetExecuteMethod = executeMethod;
            targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        // Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
        // Prism commands solve this in their implementation
        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            if (targetCanExecuteMethod != null)
                return targetCanExecuteMethod();

            if (targetExecuteMethod != null)
                return true;

            return false;
        }

        public void Execute(object parameter)
        {
            targetExecuteMethod?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        Action<T> targetExecuteMethod;
        Func<T, bool> targetCanExecuteMethod;

        public RelayCommand(Action<T> executeMethod)
        {
            targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            targetExecuteMethod = executeMethod;
            targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (targetCanExecuteMethod != null)
            {
                T tparam = (T)parameter;
                return targetCanExecuteMethod(tparam);
            }

            if (targetExecuteMethod != null)
                return true;

            return false;
        }

        // Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
        // Prism commands solve this in their implementation
        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            targetExecuteMethod?.Invoke((T)parameter);
        }
    }
}
