using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EnderEngine;

namespace DGE.Core
{
    public static class DGEModules
    {

        public static readonly List<DGEModule> modules = new List<DGEModule>();

        public static void RegisterModule(DGEModule module)
        {
            if (modules.Any(m => m.ID == module.ID))
            {
                AssemblyFramework.logger.Log($"The module {module} is already registered", Logger.LogLevel.WARN);
                return;
            }
            modules.Add(module);
            module.initCallback();
            AssemblyFramework.logger.Log($"Loaded and initialized {module}", Logger.LogLevel.INFO);
        }

    }
}
