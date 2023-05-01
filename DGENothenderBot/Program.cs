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

        private static void Main(string[] args)
        {

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyBot.module);
            DGEModules.RegisterModule(AssemblyEngine.module);
            DGEModules.RegisterModule(AssemblyNothenderBot.module);

#if DEBUG
            string configFile = "configDebug.xml"; //Running experimental config
#else
            string configFile = "config.xml"; //Running normal config
#endif

            DiscordBot bot = BotCreator.CreateBot(configFile);
            ApplicationManager.Add(bot);

            Task main = DGE.Main.Run();
            main.Wait();

        }


    }
}