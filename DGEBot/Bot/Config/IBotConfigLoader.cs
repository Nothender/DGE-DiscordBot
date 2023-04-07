using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Bot.Config
{
    public interface IBotConfigLoader
    {
        public IBotConfig LoadConfig();
    }
}
