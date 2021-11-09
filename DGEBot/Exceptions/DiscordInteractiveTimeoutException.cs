using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Exceptions
{
    public class DiscordInteractiveTimeoutException : CommandExecutionException
    {
        /// <param name="interactionSeconds">The number of seconds allowed to respond/interact</param>
        public DiscordInteractiveTimeoutException(int interactionSeconds) : base($"You didn't interact in the {interactionSeconds}s allowed") {}
        /// <param name="interactionSeconds">The number of seconds allowed to respond/interact</param>
        public DiscordInteractiveTimeoutException(string reason, int interactionSeconds) : base($"You didn't interact in the {interactionSeconds}s allowed\n - {reason}") {}

    }
}
