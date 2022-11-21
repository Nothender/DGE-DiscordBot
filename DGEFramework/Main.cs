using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DGE.Console;
using DGE.Application;
using System.Runtime.InteropServices;
using static DGE.Core.CloseEvent;
using DGE.Core.OperatingSystem;

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
        internal static bool trueStopInEx = false; // If the true stop method is being run by a more important method, to prevent out of scope errors / GC errors

        private static readonly object sender = null;

        private static CloseEvent.CtrlEventHandler handler = DefaultHandler;

        /// <summary>
        /// Inits DGE-Framework, and main app
        /// </summary>
        /// <param name="createUpdaterCommands">If true, all commands will be created</param>
        public static void Init(bool createCommands = true)
        {

#if RELEASE
            EnderEngine.Logger.IgnoreLogLevel(EnderEngine.Logger.LogLevel.DEBUG);
#endif

            OnStarting += (s, e) => AssemblyFramework.logger.Log("Starting DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStarted += (s, e) => AssemblyFramework.logger.Log("Started DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnShutdown += (s, e) => AssemblyFramework.logger.Log("Stopping DGE Main", EnderEngine.Logger.LogLevel.INFO);
            OnStopped += (s, e) => AssemblyFramework.logger.Log("Stopped DGE Main", EnderEngine.Logger.LogLevel.INFO);

            DGEModules.RegisterModule(AssemblyFramework.module);

            if(OS.CurrentOS == Core.OperatingSystem.OSPlatform.WINDOWS)
                CloseEvent.SetCloseHandler(handler, true); // Uses kernel32 stuff so it runs only on windows

            if (createCommands)
            {
                UpdaterCommands.Create();
                FrameworkCommands.Create();
            }

                TaskScheduler.UnobservedTaskException += (s, ea)
                => AssemblyFramework.logger.Log(
                    ea is null || ea.Exception is null ?
                    "An UnobservedTaskException occured, but the exception cannot be identified" : 
                    $"UnobservedTaskException caught >\n{ea.Exception.Message}\nStacktrace > {(ea.Exception.StackTrace is null ? "No stack trace" : ea.Exception.StackTrace)}\nSource > {ea.Exception.Source ?? "No source"}\nTargetSite > {ea.Exception.TargetSite?.Name ?? "No target site"}"
                    , EnderEngine.Logger.LogLevel.ERROR);

        }

        public static async Task Run(bool createUpdateInfoFile = true, MainRunMode mode = MainRunMode.CONSOLE)
        {
            OnStarting?.Invoke(sender, EventArgs.Empty);

            CloseEvent.Init(mode);

            if (createUpdateInfoFile) Updater.ProjectUpdateInfoWriter.CreateXMLFile(); //Saving information on the current running project for DGEUpdater to check for updates

            //Start code
            if (mode == MainRunMode.CONSOLE)
                _ = StartConsoleIO();

            OnStarted?.Invoke(sender, EventArgs.Empty);

            while (!stopRequest)
                await Task.Delay(50);

            TrueStop();
        }

        /// <summary>
        /// Forces the stop execution
        /// </summary>
        internal static void TrueStop()
        {

            trueStopInEx = true;

            OnShutdown?.Invoke(sender, EventArgs.Empty);

            ApplicationManager.Dispose();

            OnStopped?.Invoke(sender, EventArgs.Empty);

            Environment.Exit(0);
        }

        /// <summary>
        /// Shutdowns the entire application framework (automatically stops every app)
        /// </summary>
        public static void Stop()
        {
            if (stopRequest)
                return;
            stopRequest = true;
        }

        private static Task StartConsoleIO()
        {

            return Task.Run(async () =>
            {
                string command = "";
                string lineRead;
                string[] expression;
                string[] arguments;
                do
                {
                    lineRead = System.Console.ReadLine();
                    if (lineRead is null)
                        break;

                    AssemblyFramework.logger.Log($"[User]: \"{lineRead}\"", EnderEngine.Logger.LogLevel.INFO, EnderEngine.Logger.LogMethod.TO_FILE);

                    expression = lineRead.ToLower().Split(' ');
                    if (expression.Length <= 0)
                        continue;
                    command = expression[0].Trim(' ', '\t', '\n');
                    if (command.Length == 0)
                        continue; // Doenst run the command if nothing is written
                    arguments = null;
                    if (expression.Length > 1)
                        arguments = expression.Skip(1).ToArray();
                    
                    await Commands.ExecuteCommand(command, arguments);

                } while (command != Commands.exitCommand);
                Stop();
            });
        }

        private static bool DefaultHandler(CtrlType ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    if (trueStopInEx) // If the Framework is already true stopping we skip
                        return false;
                    TrueStop();
                    return false;
                default:
                    return false;
            }
        }

    }

    public enum MainRunMode
    {
        CONSOLE
    }


}
