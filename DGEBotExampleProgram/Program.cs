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

namespace DGE
{
    public class Program
    {
        private static void Main(string[] args)
        {
            DGE.Main.Init();

            string[] infos = File.ReadAllLines("../../../../infos.txt"); // 42

            DiscordBot bot1 = new DiscordBot(infos[0], ">");

            DiscordCommandManager.RegisterModule(typeof(Commands));
            DiscordCommandManager.RegisterModule(typeof(FunCommands));
            DiscordCommandManager.RegisterModule(typeof(ModerationCommands));
            DiscordCommandManager.RegisterModule(typeof(DebugCommands));
            DiscordCommandManager.RegisterModule(typeof(DevCommands));
            DiscordCommandManager.RegisterModule(typeof(BetaTestingCommands));

            //TODO: This will be fixed to be cleaner
            //UserFeedbackHandler.feedbackChannel = Bot1.client.GetChannelAsync(ulong.Parse(infos[1])) as ISocketMessageChannel;

            ApplicationManager.Add(bot1);

            DGE.Main.OnStarted += (s, e) => bot1.Start(); //The bot automatically starts when the app is on

            Task main = DGE.Main.Run();
            main.Wait();

            //The following command modules do not exist yet, were not reimplemented, or are deprecated / removed
            //DiscordCommandManager.RegisterModule(typeof(FrameBufferCommands));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(CommandsExemple));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(ProgramsCommands));
            //DiscordGameEngineBot.RegisterCommandModule(typeof(ApplicationServersCommands));
        }
    }
}
