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
            logger = new EnderEngine.Logger($"Process:{Id}");

            this.startInfo = startInfo;
            if (!(safeStopMethod is null))
                this.safeStopMethod = safeStopMethod;

            ApplicationManager.Add(this);
        }

        public override void Dispose()
        {
            Stop();
        }

        public override void Start()
        {
            if (!(process is null))
            {
                if (process.HasExited)
                {
                    process?.Dispose();
                    process = null;
                }
                else
                    return;
            }

            process = new Process { StartInfo = startInfo };
            process.EnableRaisingEvents = true;
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

            try
            {

                if (safeStopMethod is null)
                    process?.Kill();
                else
                    safeStopMethod(this);
            }
            catch (Exception e)
            {
                logger.Log($"ApplicationProcess (id {Id}) - Stop failed (Is the process dead already ?) : {e.Message}", EnderEngine.Logger.LogLevel.ERROR);
            }

            process?.Dispose();
            process = null;

            OnStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}