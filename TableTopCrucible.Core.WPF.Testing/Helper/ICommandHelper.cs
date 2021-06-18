using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using TableTopCrucible.Core.WPF.Testing.Exceptions;

namespace TableTopCrucible.Core.WPF.Testing.Helper
{
    public static class ICommandHelper
    {
        public static CommandActions Send(this ICommand command, string name = "<<unnamed>>")
            => new CommandActions(command, name);

    }
    public class CommandActions
    {
        private readonly ICommand _command;
        private readonly string _name;

        public CommandActions(ICommand command, string name)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _name = name;
        }

        public void Execute(object parameter = null)
        {
            if (_command.CanExecute(parameter))
                _command.Execute(parameter);
            else
                throw new ButtonDisbledException($"The command {_name} could not be executed - it is disabled");
        }
    }
}
