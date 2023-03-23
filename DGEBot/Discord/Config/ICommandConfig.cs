using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord.Config
{

    public interface IModuleConfig
    {
        /// <summary>
        /// The assembly reference name, the module manager will try to find it from this
        /// </summary>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// Simpler name for readability
        /// </summary>
        string ModuleName { get; }
    }

    public interface ICommandConfig
    {
        /// <summary>
        /// Represents the modules the bot will load/register
        /// </summary>
        IModuleConfig[] modules { get; }

    }
}
