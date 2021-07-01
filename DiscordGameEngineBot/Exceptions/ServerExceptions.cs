using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.Exceptions
{

    public class ApplicationServerException : Exception
    {
        public ApplicationServerException() :
            base("Undescribed application server error occured")
        { }

        public ApplicationServerException(string reason) :
            base($"{reason}")
        { }

        public ApplicationServerException(string reason, Exception innerException) :
            base($"{reason} - {innerException.Message}")
        { }

        public ApplicationServerException(Exception innerException) :
            base($"{innerException.Message}")
        { }
    }

    /// <summary>
    /// An exception you can throw with a reason, and a inner-exception, will be shown to the user if thrown in a command, but not reported
    /// </summary>
    [Serializable]
    public class ServerAlreadyStartedException : ApplicationServerException
    {
        public ServerAlreadyStartedException() :
            base($"Cannot start the server if it is already online")
        { }

        public ServerAlreadyStartedException(string reason) :
            base($"{reason}")
        { }

    }

    [Serializable]
    public class ServerAlreadyOfflineException : ApplicationServerException
    {
        public ServerAlreadyOfflineException() :
            base($"Cannot stop the server if it is already offline")
        { }

        public ServerAlreadyOfflineException(string reason) :
            base($"{reason}")
        { }

    }


}
