using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DGE.Console;

namespace DGE
{
    public static class Main
    {

        /// <summary>
        /// When the Start method is called
        /// </summary>
        public static event EventHandler OnStarting;
        /// <summary>
        /// When the app finished starting up
        /// </summary>
        public static event EventHandler OnStarted;
        /// <summary>
        /// When the Stop method is called
        /// </summary>
        public static event EventHandler OnShutdown;
        /// <summary>
        /// When the app has shutdown
        /// </summary>
        public static event EventHandler OnStopped;

        private static bool stopRequest = false;

        private static readonly object sender = null;

        public static void Init()
        {
            OnStarting += (s, e) => AssemblyFramework.logger.Log("Starting DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStarted += (s, e) => AssemblyFramework.logger.Log("Started DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnShutdown += (s, e) => AssemblyFramework.logger.Log("Stopping DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStopped += (s, e) => AssemblyFramework.logger.Log("Stopped DGE Main", EnderEngine.Logger.LogLevel.INFO);

            DGEModules.RegisterModule(AssemblyFramework.module);

            TaskScheduler.UnobservedTaskException += (s, ea) => AssemblyFramework.logger.Log("42" + ea.Exception.StackTrace, EnderEngine.Logger.LogLevel.ERROR); //This doesn't seem to work but whatever

        }

        public static async Task Run(MainRunMode mode = MainRunMode.CONSOLE)
        {
            
            OnStarting?.Invoke(sender, EventArgs.Empty);

            //Start code
            if (mode == MainRunMode.CONSOLE)
                _ = StartConsoleIO();
            
            OnStarted?.Invoke(sender, EventArgs.Empty);
            while (!stopRequest)
                await Task.Delay(100);
            Stop();
        }

        /// <summary>
        /// Shutdowns the entire application framework (automatically stops every app)
        /// </summary>
        public static void Stop()
        {
            if (!stopRequest)
            {
                stopRequest = true;
                return;
            }

            OnShutdown?.Invoke(sender, EventArgs.Empty);

            //Stop code

            OnStopped?.Invoke(sender, EventArgs.Empty);

        }

        private static Task StartConsoleIO()
        {
            return Task.Run(async () =>
            {
                string command = "";
                string[] expression;
                string[] arguments;
                do
                {
                    expression = System.Console.ReadLine().ToLower().Split(' ');
                    if (expression.Length < 1)
                        continue;
                    command = expression[0];
                    arguments = null;
                    if (expression.Length > 1)
                        arguments = expression.Skip(1).ToArray();
                    
                    await Commands.ExecuteCommand(command, arguments);

                } while (command != "stop");
                Stop();
            });
        }

    }

    public enum MainRunMode
    {
        CONSOLE
    }

}
