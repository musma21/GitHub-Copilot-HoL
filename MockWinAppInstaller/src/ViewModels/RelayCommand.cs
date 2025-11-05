using System;
using System.Windows.Input;

namespace MockWinAppInstaller.ViewModels
{
    /// <summary>
    /// Minimal ICommand implementation for MVVM binding.
    /// Wraps an execute delegate and optional canExecute predicate.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

    /// <summary>
    /// Determine if the command can execute for provided parameter.
    /// </summary>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    /// <summary>
    /// Invoke the underlying execute delegate.
    /// </summary>
    public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Force WPF to re-query CanExecute (use when state affecting availability changes).
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
