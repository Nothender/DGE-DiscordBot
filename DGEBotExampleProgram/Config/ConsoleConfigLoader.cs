using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;

namespace DGE.Config
{
    public class ConsoleConfigLoader : IConfigLoader
    {
        public IConfig LoadConfig()
        {
            Program.logger.Log("Creating a config. Please enter the following items in order :\n - Discord Bot token\n - Command Prefix\n - feedbackChannelId (Discord Channel ID)", Logger.LogLevel.INFO);
            string token = System.Console.ReadLine();
            Program.logger.Log($"Your discord bot token will be `{token}`", Logger.LogLevel.INFO);
            string prefix = System.Console.ReadLine();
            Program.logger.Log($"The prefix for discord commands will be `{prefix}`", Logger.LogLevel.INFO);
            string feedbackChannelId = System.Console.ReadLine();
            Program.logger.Log($"The feedbackChannelId will be `{feedbackChannelId}`", Logger.LogLevel.INFO);
            Config config = new Config(token, prefix, ulong.Parse(feedbackChannelId));
            Program.logger.Log($"Successfuly created new config", Logger.LogLevel.INFO);
            return config;
        }

    }
}
