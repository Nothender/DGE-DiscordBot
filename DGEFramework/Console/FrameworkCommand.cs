using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Console
{
    public class FrameworkCommand : ICommand
    {
        public string Name { get; }

        public string Description { get; }

        private Func<string[], string> action;

        public string Execute(string[] args) => action(args);

        public FrameworkCommand(string name, Func<string[], string> action, string description = "This command has no description")
        {
            this.action = action;
            Name = name;
            Description = description;
        }
    }
}
