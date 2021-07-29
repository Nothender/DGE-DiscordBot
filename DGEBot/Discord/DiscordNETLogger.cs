using DGE.Core;
using Discord;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static EnderEngine.Logger;

namespace DGE.Discord
{
    public static class DiscordNETLogger
    {

        public static LogLevel LogSeverityToLogLevel(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Info => LogLevel.INFO,
                LogSeverity.Verbose => LogLevel.DEBUG,
                LogSeverity.Debug => LogLevel.DEBUG,
                LogSeverity.Error => LogLevel.ERROR,
                LogSeverity.Critical => LogLevel.FATAL,
                LogSeverity.Warning => LogLevel.WARN,
                _ => LogLevel.DEBUG,
            };
        }

        public static Task Log(LogMessage message, Logger logger = null)
        {
            if (logger is null)
                logger = AssemblyBot.logger;
            logger.Log($"({message.Source}) {message.Message}", LogSeverityToLogLevel(message.Severity));
            return Task.CompletedTask;
        }

    }
}
