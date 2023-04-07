using DGE.Core;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Application
{
    public interface IApplication : IDisposable
    {
        /// <summary>
        /// The status of the application
        /// </summary>
        ApplicationStatus status { get; }

        public int Id { get; }

        public string Name { get; }

        /// <summary>
        /// When the Start method is called
        /// </summary>
        public event EventHandler OnStarting;
        /// <summary>
        /// When the app finished starting up
        /// </summary>
        public event EventHandler OnStarted;
        /// <summary>
        /// When the Stop method is called
        /// </summary>
        public event EventHandler OnShutdown;
        /// <summary>
        /// When the app has shutdown
        /// </summary>
        public event EventHandler OnStopped;

        public Logger logger { get; }

        public void Start();
        public void Stop();

        public string ToString()
        {
            return $"({GetType().Name}) Application \'{Name}\' of id {Id}, currently {status}";
        }

    }
}
