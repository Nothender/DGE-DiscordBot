using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core
{
    public class DGEModule
    {
        public readonly string NAME;
        public readonly string VERSION;

        public readonly Guid ID;

        private Action initCallback;

        private bool initialized = false;

        public DGEModule(string name, string version, Action initCallback)
        {
            NAME = name;
            VERSION = version;
            ID = Guid.NewGuid();
            this.initCallback = initCallback;
        }

        public void Init()
        {
            if (initialized)
            {
                AssemblyFramework.logger.Log($"{this} was already initialized", EnderEngine.Logger.LogLevel.WARN);
                return;
            }
            initialized = true;
            initCallback();
        }

        public override string ToString()
        {
            return $"[{NAME} {VERSION}]";
        }

    }
}
