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

        public Action initCallback;

        public DGEModule(string name, string version, Action initCallback)
        {
            NAME = name;
            VERSION = version;
            ID = Guid.NewGuid();
            this.initCallback = initCallback;
        }

        public override string ToString()
        {
            return $"[{NAME} {VERSION}]";
        }

    }
}
