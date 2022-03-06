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
using DGE.Config;

namespace DGE
{
    public class Program
    {
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
            string configFile = "config.txt" //Running normal DGE config
#endif
            //See config-exemple.txt for more information
            ConfigTextFileLoader cfgLoader = new ConfigTextFileLoader(configFile);
            IConfig config = cfgLoader.LoadConfig();

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyBot.module);
            DGEModules.RegisterModule(AssemblyEngine.module);

            DiscordBot bot1 = new DiscordBot(config.Token, config.Prefix, config.FeedbackChannelId);
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
    }
}
