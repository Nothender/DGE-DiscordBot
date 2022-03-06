using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Config
{
    public interface IConfigSaver
    {
        public void SaveConfig(IConfig config);

    }
}
