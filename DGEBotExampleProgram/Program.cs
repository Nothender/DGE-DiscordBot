using System;
using System.Threading.Tasks;
using DGE;
using DGE.Core;
using DGE.Bot;
using DGE.Application;
using System.IO;
using DGE.UI.Feedback;
using Discord.WebSocket;
using System.Security;
using DGE.Discord;
using DGE.Discord.Commands;
using DGE.Discord.Handlers;
using DGE.ProgramModules;
using DGE.Rendering;
using System.Drawing;
using System.Diagnostics;
using static DGE.Core.CloseEvent;
using DGE.Discord.Config;
using EnderEngine;
using Discord;

namespace DGE
{
    public static class Program
    {

        internal static Logger logger = new Logger("MainProgram");

        private static void Main(string[] args)
        {
            DiscordBotMain();
        }

        private static void DiscordBotMain()
        {
            //TODO: This will be fixed to be cleaner
#if DEBUG
            string configFile = "config-exp.txt"; //Running experimental config
#else
            string configFile = "config.txt"; //Running normal DGE config
#endif

            IConfig config = LoadBotConfig(configFile);

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All ^ GatewayIntents.GuildPresences ^ GatewayIntents.GuildInvites ^ GatewayIntents.GuildScheduledEvents
            };

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyBot.module);
            DGEModules.RegisterModule(AssemblyEngine.module);

            DiscordBot bot1 = new DiscordBot(config, socketConfig);
            ApplicationManager.Add(bot1);

            bot1.OnStarted += (s, e) => ProgramModule.RestoreSavedPrograms(bot1);

            DGE.Main.OnStarted += (s, e) => bot1.Start(); //The bot automatically starts when the app is on

            bot1.RegisterCommandModule(typeof(DevCommands));
            bot1.RegisterCommandModule(typeof(DebugCommands));
            bot1.RegisterCommandModule(typeof(Commands));
            bot1.RegisterCommandModule(typeof(FunCommands));
            bot1.RegisterCommandModule(typeof(ModerationCommands));
            bot1.RegisterCommandModule(typeof(BetaTestingCommands));
            bot1.RegisterCommandModule(typeof(FrameBufferCommands));
            bot1.RegisterCommandModule(typeof(ProgramsCommands));
            bot1.RegisterCommandModule(typeof(FractalCommands));

            Task main = DGE.Main.Run();
            main.Wait();

            //The following command modules do not exist yet, were not reimplemented, or are deprecated / removed
            //DiscordGameEngineBot.RegisterCommandModule(typeof(CommandsExemple));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(ApplicationServersCommands));
        }

        private static IConfig LoadBotConfig(string configFile)
        {
            logger.Log($"Loading config from `./{configFile}`", Logger.LogLevel.INFO);
            IConfigLoader cfgLoader;
            IConfig config;
            //See config-exemple.txt for more information
            if (File.Exists(configFile))
            {
                cfgLoader = new ConfigTextFileParser(configFile);
                config = cfgLoader.LoadConfig();
            }
            else
            {
                logger.Log("The program wasn't able to identify a valid config file\nAutomatically running console config file creation process", Logger.LogLevel.WARN);
                cfgLoader = new ConsoleConfigLoader(logger);
                config = cfgLoader.LoadConfig();

                IConfigSaver cfgSaver = new ConfigTextFileParser(configFile);
                cfgSaver.SaveConfig(config);
                logger.Log("Saved newly created config", Logger.LogLevel.INFO);
            }
            return config;
        }

    }
}
