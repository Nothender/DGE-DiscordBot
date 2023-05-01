using DGE.Application;
using DGE.Bot;
using DGE.Bot.Config;
using DGE.Core;
using Discord;
using Discord.WebSocket;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE
{
    public static class BotCreator
    {

        public static DiscordBot CreateBot(string configPath)
        {
            IBotConfig config = LoadBotConfig(configPath);

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All ^ GatewayIntents.GuildPresences ^ GatewayIntents.GuildInvites ^ GatewayIntents.GuildScheduledEvents
            };

            DiscordBot bot = new DiscordBot();
            bot.Load(config, socketConfig);

            return bot;
        }

        public static IBotConfig LoadBotConfig(string configFile)
        {
            AssemblyNothenderBot.logger.Log($"Loading config from `./{configFile}`", Logger.LogLevel.INFO);
            IBotConfigLoader cfgLoader;
            IBotConfig config;
            //See config-exemple.txt for more information
            if (File.Exists(configFile))
            {
                try
                {
                    cfgLoader = new BotConfigXMLFileParser(configFile);
                    config = cfgLoader.LoadConfig();
                    return config;
                }
                catch (Exception e)
                {
                    AssemblyNothenderBot.logger.Log($"Exception occured whilst trying to load config from file : {e.Message}", Logger.LogLevel.ERROR);
                }
            }
            AssemblyNothenderBot.logger.Log("The program wasn't able to identify a valid config file\nAutomatically running console config file creation process", Logger.LogLevel.WARN);

            cfgLoader = new ConsoleBotConfigLoader(AssemblyNothenderBot.logger);
            config = cfgLoader.LoadConfig();

            IBotConfigSaver cfgSaver = new BotConfigXMLFileParser(configFile);
            cfgSaver.SaveConfig(config);
            AssemblyNothenderBot.logger.Log($"Saved newly created config (as '{configFile}')", Logger.LogLevel.INFO);

            return config;
        }

    }
}
