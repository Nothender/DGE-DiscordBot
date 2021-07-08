using DGE.Core;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Discord
{
    public static class DiscordCommandManager
    {
        public static CommandService commands;

        static DiscordCommandManager()
        {
            commands = new CommandService();
        }

        public static void RegisterModule(Type type)
        {
            commands.AddModuleAsync(type, null); //TODO: not clean and may cause bugs, per Bot instance client services ?
            AssemblyBot.logger.Log($"Loaded command module [{type.Name}]", EnderEngine.Logger.LogLevel.INFO);
        }

    }
}
