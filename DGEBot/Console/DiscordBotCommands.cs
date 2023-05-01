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
            Commands.AddCommand(new FrameworkCommand("botregc", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("registerbotcommands", 1, a.Length, true);
                int appId = int.Parse(a[0]);
                bool deleteMissing = true;
                if (a.Length > 1)
                    deleteMissing = bool.Parse(a[1]);
                DiscordBot app = ApplicationManager.Get(appId) as DiscordBot;
                if (app is null)
                    throw new Exception("Application is not of type `DiscordBot`");
                app.RegisterCommands(deleteMissing).Wait();
                return null;
            }, "Registers loaded command modules (slash commands) to guild/globally depending on debug mode", 
            new string[] { "appId > Framework application id of the bot", "(optional) deleteMissing > Deletes modules that are not registered anymore (default true)" }));
        }
    }
}
