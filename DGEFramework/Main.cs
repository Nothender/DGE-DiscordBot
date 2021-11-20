using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DGE.Console;
using DGE.Application;

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

        /// <summary>
        /// Inits DGE-Framework, and main app
        /// </summary>
        /// <param name="createUpdaterCommands">If true, commands related to the updater will be created</param>
        public static void Init(bool createUpdaterCommands = true)
        {

#if RELEASE
            EnderEngine.Logger.IgnoreLogLevel(EnderEngine.Logger.LogLevel.DEBUG);
#endif

            OnStarting += (s, e) => AssemblyFramework.logger.Log("Starting DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStarted += (s, e) => AssemblyFramework.logger.Log("Started DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnShutdown += (s, e) => AssemblyFramework.logger.Log("Stopping DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStopped += (s, e) => AssemblyFramework.logger.Log("Stopped DGE Main", EnderEngine.Logger.LogLevel.INFO);

            DGEModules.RegisterModule(AssemblyFramework.module);

            FrameworkCommands.Create();
            if (createUpdaterCommands) UpdaterCommands.Create();

            TaskScheduler.UnobservedTaskException += (s, ea)
                => AssemblyFramework.logger.Log(
                    ea is null ?
                    "An UnobservedTaskException occured, but the exception cannot be identified" : 
                    $"UnobservedTaskException caught >\n{ea.Exception.Message}\nStacktrace >{(ea.Exception.StackTrace is null ? "No stack trace" : ea.Exception.StackTrace)}\nSource > {ea.Exception.Source ?? "No source"}\nTargetSite > {ea.Exception.TargetSite?.Name ?? "No target site"}"
                    , EnderEngine.Logger.LogLevel.ERROR);

        }

        public static async Task Run(MainRunMode mode = MainRunMode.CONSOLE)
        {
            
            OnStarting?.Invoke(sender, EventArgs.Empty);
            
            ProjectUpdateInfoWriter.CreateXMLFile(); //Saving information on the current running project for DGEUpdater to check for updates

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

            ApplicationManager.Dispose();

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

                } while (command != "exit");
                Stop();
            });
        }

    }

    public enum MainRunMode
    {
        CONSOLE
    }

}
