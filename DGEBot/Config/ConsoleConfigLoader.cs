using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;

namespace DGE.Discord.Config
{
    public class ConsoleConfigLoader : IConfigLoader
    {

        private Logger programLogger;

        public ConsoleConfigLoader(Logger programLogger)
        {
            this.programLogger = programLogger;
        }

        public IConfig LoadConfig()
        {
            programLogger.Log("Creating a config. Please enter the following items in order :\n - Discord Bot token\n - Command Prefix\n - feedbackChannelId (Discord Channel ID)", Logger.LogLevel.INFO);
            string token = System.Console.ReadLine();
            programLogger.Log($"Your discord bot token will be `{token}`", Logger.LogLevel.INFO);
            string prefix = System.Console.ReadLine();
            programLogger.Log($"The prefix for discord commands will be `{prefix}`", Logger.LogLevel.INFO);
            string feedbackChannelId = System.Console.ReadLine();
            programLogger.Log($"The feedbackChannelId will be `{feedbackChannelId}`", Logger.LogLevel.INFO);
            Config config = new Config(token, prefix, ulong.Parse(feedbackChannelId));
            programLogger.Log($"Successfuly created new config", Logger.LogLevel.INFO);
            return config;
        }

    }
}
