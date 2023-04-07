using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;

namespace DGE.Application
{
    public abstract class ApplicationBase : IApplication
    {

        public int Id { get; internal set; }
        public string Name { get; protected set; }

        public ApplicationStatus status { get; protected set; }

        public Logger logger { get; protected set; }

        public abstract event EventHandler OnStarting;
        public abstract event EventHandler OnStarted;
        public abstract event EventHandler OnShutdown;
        public abstract event EventHandler OnStopped;

        private bool storageSetup = false;

        public ApplicationBase(string name)
        {
            OnStarting += (s, e) => status = ApplicationStatus.STARTING;
            OnStarted += (s, e) => status = ApplicationStatus.ON;
            OnShutdown += (s, e) => status = ApplicationStatus.STOPPING;
            OnStopped += (s, e) => status = ApplicationStatus.OFF;
            Name = name;
        }

        public void SetupApplicationStorage()
        {
            string name = $"{Name}Storage";
            if (!Paths.Exists(name))
                Paths.Add(name, Paths.Get("Storage") + $"{Name}/");
            storageSetup = true;
        }

        public string GetStoragePath()
        {
            if (!storageSetup)
                logger.Log("Storage for this application is not setup yet, resulting to default storage path", Logger.LogLevel.WARN);
            return Paths.Get($"{Name}Storage");
        }

        public abstract void Dispose();

        public abstract void Start();
        public abstract void Stop();
    }
}
