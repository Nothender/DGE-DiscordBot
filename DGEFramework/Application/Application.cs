using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Application
{
    public abstract class Application : IApplication
    {

        public int Id { get; internal set; }

        public ApplicationStatus status { get; protected set; }

        public Logger logger { get; protected set; }

        public abstract event EventHandler OnStarting;
        public abstract event EventHandler OnStarted;
        public abstract event EventHandler OnShutdown;
        public abstract event EventHandler OnStopped;

        public abstract void Dispose();

        public abstract void Start();
        public abstract void Stop();
    }
}
