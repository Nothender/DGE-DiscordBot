using System;
using System.Threading.Tasks;
using Discord;

namespace DiscordGameEngine.Core
{
    public static class LogManager
    {
        public const string DGE_ERROR = "(X) [DGE_ERROR]: ";
        public const string DGE_DEBUG = "-- [DGE_DEBUG]: ";
        public const string DGE_WARN = "/!\\ [DGE_WARN]: ";
        public const string DGE_LOG = "[DGE_LOG]: ";

        /// <summary>
        /// Called at each log event from discord : used to debug
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        internal static Task LogDebug(LogMessage msg)
        {
            EnderEngine.Logger.LogLevel logLevel;
            switch (msg.Severity)
            {
                case LogSeverity.Info:
                    logLevel = EnderEngine.Logger.LogLevel.INFO;
                    break;
                case LogSeverity.Verbose:
                    logLevel = EnderEngine.Logger.LogLevel.DEBUG;
                    break;
                case LogSeverity.Debug:
                    logLevel = EnderEngine.Logger.LogLevel.DEBUG;
                    break;
                case LogSeverity.Error:
                    logLevel = EnderEngine.Logger.LogLevel.ERROR;
                    break;
                case LogSeverity.Critical:
                    logLevel = EnderEngine.Logger.LogLevel.FATAL;
                    break;
                case LogSeverity.Warning:
                    logLevel = EnderEngine.Logger.LogLevel.WARN;
                    break;
                default:
                    logLevel = EnderEngine.Logger.LogLevel.DEBUG;
                    break;
            }
            DGEMain.DGELogger.Log($"({msg.Source}) {msg.Message}", logLevel);
            return Task.CompletedTask;
        }

    }
}
