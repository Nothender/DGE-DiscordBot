using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Bot.Config
{
    /// <summary>
    /// Config interface for Discord Command Modules
    /// </summary>
    public interface ICommandModuleConfig
    {
        /// <summary>
        /// The assembly reference name, the module manager will try to find it from this
        /// </summary>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// Simpler name for readability
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Whether the command module should be registered only when debugging, only in the set debug guild
        /// </summary>
        bool DebugOnly { get; }
    }
}
