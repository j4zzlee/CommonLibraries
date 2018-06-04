using System;
using System.Collections.Generic;

namespace Commands
{
    public interface ICommand
    {
        string CommandName { get; }
        string Description { get; }
        ICommand ReadArguments(IEnumerable<string> args);
        void Execute();
        void Help();
    }
}