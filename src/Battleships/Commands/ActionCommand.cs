using BusinessLayer;
using BusinessLayer.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Battleships.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecuteAction;

        public ActionCommand(Action action) : this(action, () => true)
        {
        }

        public ActionCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecuteAction = canExecute;
        }

        public Guid Guid { get; set; }
        public Field Field { get; set; }

        public void Execute(object obj)
        {
            _action();
        }

        public bool CanExecute(object obj)
        {
            return _canExecuteAction();
        }

        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value;
            } 
        }
    }
}
