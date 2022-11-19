using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Discord.Config
{
    public interface IConfigLoader
    {
        public IConfig LoadConfig();
    }
}
