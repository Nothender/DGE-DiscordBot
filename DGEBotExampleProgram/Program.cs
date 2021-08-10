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
using DiscordGameEngine.UI.Commands;
using DGE.Discord;
using DGE.Discord.Commands;
using DGE.Discord.Handlers;
using DGE.ProgramModules;

namespace DGE
{

    public class Program
    {
        private static void Main(string[] args)
        {

            //TODO: This will be fixed to be cleaner
#if DEBUG
            string[] infos = File.ReadAllLines("config-exp.txt"); //Running experimental config
#else
            string[] infos = File.ReadAllLines("config.txt"); //Running normal DGE config
#endif
            //See config-exemple.txt for more information

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyBot.module);
            DGEModules.RegisterModule(AssemblyEngine.module);

            DiscordCommandManager.RegisterModule(typeof(Commands));
            DiscordCommandManager.RegisterModule(typeof(FunCommands));
            DiscordCommandManager.RegisterModule(typeof(ModerationCommands));
            DiscordCommandManager.RegisterModule(typeof(DebugCommands));
            DiscordCommandManager.RegisterModule(typeof(DevCommands));
            DiscordCommandManager.RegisterModule(typeof(BetaTestingCommands));
            DiscordCommandManager.RegisterModule(typeof(FrameBufferCommands));
            DiscordCommandManager.RegisterModule(typeof(ProgramsCommands));

            DiscordBot bot1 = new DiscordBot(infos[0], "<", ulong.Parse(infos[1]));
            ApplicationManager.Add(bot1);
            DGE.Main.OnStarted += (s, e) => bot1.Start(); //The bot automatically starts when the app is on
            bot1.OnStarted += (s, e) => ProgramModule.RestoreSavedPrograms(bot1);

            Task main = DGE.Main.Run();
            main.Wait();

            //The following command modules do not exist yet, were not reimplemented, or are deprecated / removed
            //DiscordGameEngineBot.RegisterCommandModule(typeof(CommandsExemple));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(ApplicationServersCommands));
        }
    }
}
