using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Updater
{
    /// <summary>
    /// Contains the tags necessary to communicate from Updater to DGE
    /// </summary>
    public static class UpdaterTags
    {
        /// <summary>
        /// This tag is used as prefix for other tags, to know if the message is for important information
        /// </summary>
        public static readonly string PassthroughInfo = "DGEUT";
        /// <summary>
        /// This tag is used as a prefix, so DGE knows it has to log 
        /// </summary>
        internal static readonly string log = "DGELT-";
        /// <summary>
        /// Returns the log tag used to write logging info, that will be logged by DGE
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static string GetLogTag(EnderEngine.Logger.LogLevel logLevel)
        {
            return $"{log}{(int)logLevel}";
        }

        //These Tags are added after the `PassthroughInfo` one
        //No need for summary its quite self explanatory (at least i hope for future me)
        public static readonly string UpdateAvailableTag = "UA";
        public static readonly string UpdateDownloadedTag = "UD";
        public static readonly string Stopped = "SU";
    }
}
