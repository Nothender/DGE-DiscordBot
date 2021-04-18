using System;
using System.Collections.Generic;
using System.Text;
using DiscordGameEngine.Services;

namespace DiscordGameEngine.Exceptions
{
    /// <summary>
    /// An exception you can throw with a reason, and a inner-exception, will be shown to the user if thrown in a command, but not reported
    /// </summary>
    [Serializable]
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException() : 
            base($"{CommandHandler.AvoidBugReportErrorTag}Undescribed error occured while executing the command") { }

        public CommandExecutionException(string reason) : 
            base($"{CommandHandler.AvoidBugReportErrorTag}{reason}") { }

        public CommandExecutionException(string reason, Exception innerException) :
            base($"{CommandHandler.AvoidBugReportErrorTag}{reason} - {innerException.Message}") { }

        public CommandExecutionException(Exception innerException) :
            base($"{CommandHandler.AvoidBugReportErrorTag}{innerException.Message}") { }
    }
}
