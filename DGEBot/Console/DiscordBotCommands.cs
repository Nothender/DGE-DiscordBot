using DGE.Application;
using DGE.Bot;
using DGE.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Console
{
    public static class DiscordBotCommands
    {
        public static void CreateCommands()
        {
            Commands.AddCommand(new FrameworkCommand("registerbotcommands", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("registerbotcommands", 1, a.Length, true);
                int appId = int.Parse(a[0]);
                bool deleteMissing = true;
                if (appId > 1)
                    deleteMissing = bool.Parse(a[1]);
                DiscordBot app = ApplicationManager.Get(appId) as DiscordBot;
                if (app is null)
                    throw new Exception("Application is not of type `DiscordBot`");
                app.RegisterCommands(deleteMissing).Wait();
                return "Attempted (success ?) to register command modules from discord bot application : " + a[0];
            }, "Registers loaded command modules (slash commands) to guild/globally depending on config, args : app id, (optional) deleteMissing"));
        }
    }
}
