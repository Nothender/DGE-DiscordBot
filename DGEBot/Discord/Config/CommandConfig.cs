using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord.Config
{

    public class ModuleConfig : IModuleConfig
    {

        public string AssemblyQualifiedName { get; }

        public string ModuleName { get; }

        public ModuleConfig(string assemblyQualifiedName, string moduleName)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
            ModuleName = moduleName;
        }
    }

    public class CommandConfig : ICommandConfig
    {
        public IModuleConfig[] modules { get; }

        public CommandConfig(ModuleConfig[] modules) { this.modules = modules; }

    }
}
