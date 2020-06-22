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
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}
