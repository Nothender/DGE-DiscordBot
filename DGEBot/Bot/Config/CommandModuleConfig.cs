using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Bot.Config
{
    public class CommandModuleConfig : ICommandModuleConfig
    {
        public string AssemblyQualifiedName { get; }

        public string ModuleName { get; }

        public bool DebugOnly { get; }

        public CommandModuleConfig(string assemblyQualifiedName, string moduleName, bool debugOnly = false)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
            ModuleName = moduleName;
            DebugOnly = debugOnly;
        }

        /// <summary>
        /// Alternative CommandModuleConfig constructor
        /// </summary>
        /// <param name="moduleName">The module name that should show up</param>
        /// <param name="namespacePath">The namespace the module class shows up in</param>
        /// <param name="className">The module's class name</param>
        /// <param name="assemblyName">The assembly (C# Project) the class is from</param>
        public CommandModuleConfig(string moduleName, string namespacePath, string className, string assemblyName, bool debugOnly = false)
        {
            ModuleName = moduleName;
            AssemblyQualifiedName = namespacePath + "." + className + "," + assemblyName;
            DebugOnly = debugOnly;
        }

    }
}
