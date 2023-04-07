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
            programLogger.Log("Creating a config. Please enter the following items in order :\n - Discord Bot Token\n - Debug Guild Id\n - Feedback Channel Id (Discord Channel ID)\n(Optional) Discord Command Modules to include", Logger.LogLevel.INFO);
            
            string token = System.Console.ReadLine().Trim();
            programLogger.Log($"Your discord bot token will be `{token}`", Logger.LogLevel.INFO);

            GetUlong(out ulong debugGuildId);
            programLogger.Log($"The debug guild id for discord commands will be `{debugGuildId}`", Logger.LogLevel.INFO);

            GetUlong(out ulong feedbackChannelId);
            programLogger.Log($"The feedbackChannelId will be `{feedbackChannelId}`", Logger.LogLevel.INFO);

            BotConfig config = new BotConfig(token, debugGuildId, feedbackChannelId, GetCommandModuleConfigs());
            programLogger.Log($"Successfuly created new config", Logger.LogLevel.INFO);

            return config;
        }

        private void GetUlong(out ulong u)
        {
            while (!ulong.TryParse(System.Console.ReadLine(), out u))
                programLogger.Log("Cannot understand value, type must be ulong (uint64)", Logger.LogLevel.WARN);
        }

        private void GetInt(out int i)
        {
            while (!int.TryParse(System.Console.ReadLine(), out i))
                programLogger.Log("Cannot understand value, type must be ulong (int32)", Logger.LogLevel.WARN);
        }

        private void GetYesNo(out bool y)
        {
            string s;
            while (true)
            {
                s = System.Console.ReadLine().Trim().ToLower();
                if (s == "y" || s == "yes")
                {
                    y = true;
                    return;
                }
                else if (s == "n" || s == "no")
                {
                    y = false;
                    return;
                }
            }
        }

        private ICommandModuleConfig[] GetCommandModuleConfigs()
        {
            programLogger.Log("How many Command Modules do you wish to add to your bot ?", Logger.LogLevel.INFO);
            int qtt;
            GetInt(out qtt);
            ICommandModuleConfig[] modules = new ICommandModuleConfig[qtt];
            if (qtt == 0)
            {
                programLogger.Log("Skipping command modules configuration (Check commands to later add them)", Logger.LogLevel.INFO);
            }
            programLogger.Log("Do you have the AssemblyQualifiedName for every module ? (y/n)", Logger.LogLevel.INFO);
            bool hasAsmQualName;
            bool debugOnly;
            GetYesNo(out hasAsmQualName);
            string[] values = new string[4];
            for (int i = 0; i < qtt; i++)
            {
                programLogger.Log($"CommandModule {i} - Please write the shortened module name :", Logger.LogLevel.INFO);
                values[0] = System.Console.ReadLine().Trim();
                programLogger.Log("Is this module to be registered only when debugging (in debug guild) ? (y/n)", Logger.LogLevel.INFO);
                GetYesNo(out debugOnly);
                if (hasAsmQualName)
                {
                    programLogger.Log("Write AssemblyQualifiedName :", Logger.LogLevel.INFO);
                    values[1] = System.Console.ReadLine().Trim();
                    modules[i] = new CommandModuleConfig(values[0], values[1], debugOnly);
                }
                else
                {
                    programLogger.Log("Write the class name of the module :", Logger.LogLevel.INFO);
                    values[1] = System.Console.ReadLine().Trim();
                    programLogger.Log("Write the full namespace of the class :", Logger.LogLevel.INFO);
                    values[2] = System.Console.ReadLine().Trim();
                    programLogger.Log("Write the assembly name (C# Project location) of the class :", Logger.LogLevel.INFO);
                    values[3] = System.Console.ReadLine().Trim();
                    modules[i] = new CommandModuleConfig(values[0], values[2], values[1], values[3], debugOnly);
                }
            }
            return modules;
        } 

    }
}
