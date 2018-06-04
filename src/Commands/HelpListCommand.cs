using System;
using System.Collections.Generic;
using ConsoleExtensions;

namespace Commands
{
    public class HelpListCommand : Command
    {
        private readonly IList<ICommand> _availableCommands;
        public HelpListCommand(IList<ICommand> availableCommands)
        {
            _availableCommands = availableCommands ?? new List<ICommand>();
        }

        public override string CommandName => "commands:list";
        public override string Description => "List all commands available";

        protected override void OnExecute()
        {
            Help();
        }

        protected override ICommand Filter()
        {
            return this;
        }
        
        public override void Help()
        {
            "List of commands available:".PrettyPrint(ConsoleColor.White);
            foreach (var availableCommand in _availableCommands)
            {
                $"\t{availableCommand.CommandName}{{t:40}}{availableCommand.Description}".PrettyPrint(ConsoleColor.White);
            }
            "* Type {f:Green}[command] --help{f:d} for more information".PrettyPrint(ConsoleColor.White);
        }
    }
}