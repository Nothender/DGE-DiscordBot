using DiscordGameEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public abstract class ApplicationServer : IApplicationServer
    {
        public ApplicationServerState status { get; protected set; } = ApplicationServerState.OFFLINE;
        public bool isOnline => status == ApplicationServerState.ONLINE;
        public string displayName { get; set; }
        public string address { get; set; }
        public string typeName { get; protected set; }

        public ApplicationServer(string address, string displayName = null)
        {
            this.address = address;
            if (displayName is null)
                displayName = address;
            this.displayName = displayName;

            DiscordGameEngineBot.OnShutdown += StopOnShutdown;
        }

        public abstract void Start();
        public abstract void Stop();

        protected void Started()
        {
            status = ApplicationServerState.ONLINE;
            IApplicationServer.IStarted(this, new EventArgs());
        }

        protected void Stopped()
        {
            status = ApplicationServerState.OFFLINE;
            IApplicationServer.IStopped(this, new EventArgs());
        }

        private void StopOnShutdown(object sender, EventArgs e)
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                if (!(ex is ApplicationServerException))
                    throw ex;
            }
        }

    }
}
