using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DGE;
using DGE.Core;
using DGE.Core.OperatingSystem;

namespace DGE.Core
{
    /// <summary>
    /// Handles correct closing of the application
    /// </summary>
    public static class CloseEvent
    {

        public static EventHandler naiveHandler;

        /// <summary>
        /// Initialization doesn't need to be at Init but can be at run
        /// </summary>
        /// <param name="runMode"></param>
        internal static void Init(MainRunMode runMode)
        {
            if (OS.CurrentOS == OperatingSystem.OSPlatform.UNIX)
            {
                if (runMode == MainRunMode.CONSOLE)
                    System.Console.CancelKeyPress += (s, e) => naiveHandler?.Invoke(s, e);
                AppDomain.CurrentDomain.ProcessExit += naiveHandler;
            }
        }

        public static bool SetCloseHandler(CtrlEventHandler handler, bool add)
        {
            if (OS.CurrentOS == OperatingSystem.OSPlatform.WINDOWS)
                return WindowsCloseHandler.SetConsoleCtrlHandler(handler, add);
            else if (OS.CurrentOS == OperatingSystem.OSPlatform.UNIX)
                naiveHandler += (s, e) => handler.Invoke(CtrlType.CTRL_CLOSE_EVENT); //Default CtrlType event
            return false;
        }

        public delegate bool CtrlEventHandler(CtrlType ctrlType);

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}
