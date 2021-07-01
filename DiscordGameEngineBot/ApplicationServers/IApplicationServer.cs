using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public interface IApplicationServer
    {
        string displayName { get; set; }
        string address { get; set; }
        string typeName { get; }

        static event EventHandler OnStarted;
        static event EventHandler OnStopped;

        ApplicationServerState status { get; }

        static void IStarted(object sender, EventArgs e)
        {
            OnStarted?.Invoke(sender, e);
        }

        static void IStopped(object sender, EventArgs e)
        {
            OnStopped?.Invoke(sender, e);
        }

        void Start();
        void Stop();

    }
}
