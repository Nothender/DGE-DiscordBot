using System;
using System.Collections.Generic;
using System.Text;
using DGE;
using DGE.Application;
using DGE.Core;
using System.Diagnostics;

namespace DGE.Processes
{
    public class ApplicationProcess : ApplicationBase
    {
        #region Application
        public override event EventHandler OnStarting;
        public override event EventHandler OnStarted;
        /// <summary>
        /// Event fired when the user Runs the Stop method -> will not run if the process is shutting down by itself.
        /// </summary>
        public override event EventHandler OnShutdown;
        public override event EventHandler OnStopped;
        #endregion Application

        private readonly Action<ApplicationProcess> safeStopMethod;

        public ProcessStartInfo startInfo;

        public Process process;

        public ApplicationProcess(ProcessStartInfo startInfo, Action<ApplicationProcess> safeStopMethod = null) : base()
        {
            this.startInfo = startInfo;
            if (!(safeStopMethod is null))
                this.safeStopMethod = safeStopMethod;

            ApplicationManager.Add(this);

            logger = new EnderEngine.Logger($"Process:{Id}");
        }

        public override void Dispose()
        {
            Stop();
        }

        public override void Start()
        {
            if (!ProcessExitedCleanUp() || status >= ApplicationStatus.ON)
                return;

            process = new Process { StartInfo = startInfo };
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => ProcessExitedCleanUp();
            process.Exited += OnStopped;

            OnStarting?.Invoke(this, EventArgs.Empty);

            try
            {
                process.Start();
            }
            catch
            {
                OnStopped?.Invoke(this, EventArgs.Empty);
                return;
            }
            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public override void Stop()
        {
            OnShutdown?.Invoke(this, EventArgs.Empty);

            if (ProcessExitedCleanUp() || status <= ApplicationStatus.STOPPING) // If the process quit already // Force stop ? (the application may show "off" whilst it is endlessly shutting down)
                return;

            try
            {
                if (safeStopMethod is null)
                    process?.Kill();
                else
                {
                    logger.Log($"Invoking existing process SafeStop method", EnderEngine.Logger.LogLevel.INFO);
                    safeStopMethod(this);
                }
            }
            catch (Exception e)
            {
                logger.Log($"Stop failed (Is the process exited already ?) : {e.Message}", EnderEngine.Logger.LogLevel.ERROR);
            }

            ProcessExitedCleanUp();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Cleans up pocess memory usage if it has exited
        /// </summary>
        /// <returns> Returns if it could clean up memory / or it is already cleaned up, false if it couldn't (could mean the process is still running) </returns>
        private bool ProcessExitedCleanUp() //This function is used redondently to prevent bugs
        {
            if (process is null)
                return true;
            if (process.HasExited)
            {
                process?.Dispose();
                process = null;
                return true;
            }
            return false;
        }

    }
}