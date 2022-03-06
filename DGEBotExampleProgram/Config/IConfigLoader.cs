using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Config
{
    public interface IConfigLoader
    {
        public IConfig LoadConfig();

        public void SaveConfig(IConfig config);

    }
}
