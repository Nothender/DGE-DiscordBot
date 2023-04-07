using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Console
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Execute(string[] args);
        public string[] ArgumentNameDescriptions { get; }
    }
}
