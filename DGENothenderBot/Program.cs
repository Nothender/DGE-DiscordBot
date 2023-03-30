using DGE.Application;
using DGE.Bot;
using DGE.Bot.Config;
using DGE.Core;
using DGE.Discord;
using Discord;
using Discord.WebSocket;
using EnderEngine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DGE
{
    public static class Program
    {
        public static Logger logger = new Logger("MainProgram");

        private static void Main(string[] args)
        {
            DiscordBotMain();
        }

        private static void DiscordBotMain()
        {

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyBot.module);
            DGEModules.RegisterModule(AssemblyEngine.module);

            CreateBot();

            Task main = DGE.Main.Run();
            main.Wait();

            //The following command modules do not exist yet, were not reimplemented, or are deprecated / removed
            //DiscordGameEngineBot.RegisterCommandModule(typeof(CommandsExemple));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(ApplicationServersCommands));
        }

        private static void CreateBot()
        {
            //TODO: This will be fixed to be cleaner
#if DEBUG
            string configFile = "configDebug.xml"; //Running experimental config
#else
            string configFile = "config.xml"; //Running normal DGE config
#endif

            IBotConfig config = LoadBotConfig(configFile);

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All ^ GatewayIntents.GuildPresences ^ GatewayIntents.GuildInvites ^ GatewayIntents.GuildScheduledEvents
            };

            DiscordBot bot1 = new DiscordBot("DGEExample");
            bot1.Load(config, socketConfig);
            ApplicationManager.Add(bot1);


            //bot1.LoadModuleConfig(new CommandConfig(new ModuleConfig[1] { new ModuleConfig("DGE.Discord.Commands.Commands,DGEBot", "BaseCommands") }));

            //bot1.RegisterCommandModule(typeof(DevCommands));
            //bot1.RegisterCommandModule(typeof(DebugCommands));
            //bot1.LoadCommandModule(typeof(DGE.Discord.Commands.Commands));
            //bot1.RegisterCommandModule(typeof(FunCommands));
            //bot1.RegisterCommandModule(typeof(ModerationCommands));
            //bot1.RegisterCommandModule(typeof(BetaTestingCommands));
            //bot1.RegisterCommandModule(typeof(FrameBufferCommands));
            //bot1.RegisterCommandModule(typeof(ProgramsCommands));
            //bot1.RegisterCommandModule(typeof(FractalCommands));

            // bot1.OnStarted += (s, e) => ProgramModule.RestoreSavedPrograms(bot1);

            // DGE.Main.OnStarted += (s, e) => bot1.Start(); //The bot automatically starts when the app is on
        }

        private static IBotConfig LoadBotConfig(string configFile)
        {
            logger.Log($"Loading config from `./{configFile}`", Logger.LogLevel.INFO);
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
                    logger.Log($"Exception occured whilst trying to load config from file : {e.Message}", Logger.LogLevel.ERROR);
                }
            }
            logger.Log("The program wasn't able to identify a valid config file\nAutomatically running console config file creation process", Logger.LogLevel.WARN);

            cfgLoader = new ConsoleBotConfigLoader(logger);
            config = cfgLoader.LoadConfig();

            IBotConfigSaver cfgSaver = new BotConfigXMLFileParser(configFile);
            cfgSaver.SaveConfig(config);
            logger.Log($"Saved newly created config (as '{configFile}')", Logger.LogLevel.INFO);

            return config;
        }
    }
}