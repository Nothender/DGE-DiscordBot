using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;

namespace DGE.Bot.Config
{
    public class ConsoleBotConfigLoader : IBotConfigLoader
    {

        private Logger programLogger;

        public ConsoleBotConfigLoader(Logger programLogger)
        {
            this.programLogger = programLogger;
        }

        public IBotConfig LoadConfig()
        {
            void GetUlong(out ulong u)
            {
                while (!ulong.TryParse(System.Console.ReadLine(), out u))
                    programLogger.Log("Cannot understand value, type must be ulong (int64)", Logger.LogLevel.WARN);
            }

            programLogger.Log("Creating a config. Please enter the following items in order :\n - Discord Bot Token\n - Debug Guild Id\n - Feedback Channel Id (Discord Channel ID)", Logger.LogLevel.INFO);
            
            string token = System.Console.ReadLine().Trim();
            programLogger.Log($"Your discord bot token will be `{token}`", Logger.LogLevel.INFO);

            GetUlong(out ulong debugGuildId);
            programLogger.Log($"The prefix for discord commands will be `{debugGuildId}`", Logger.LogLevel.INFO);

            GetUlong(out ulong feedbackChannelId);
            programLogger.Log($"The feedbackChannelId will be `{feedbackChannelId}`", Logger.LogLevel.INFO);

            BotConfig config = new BotConfig(token, debugGuildId, feedbackChannelId);
            programLogger.Log($"Successfuly created new config (filename : configFile)", Logger.LogLevel.INFO);
            return config;
        }

    }
}
