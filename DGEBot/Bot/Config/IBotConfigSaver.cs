using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Bot.Config
{
    public interface IBotConfigSaver
    {
        public void SaveConfig(IBotConfig config);

    }
}
