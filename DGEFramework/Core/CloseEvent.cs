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
        public static bool SetCloseHandler(CtrlEventHandler handler, bool add)
        {
            if (OS.CurrentOS == OperatingSystem.OSPlatform.WINDOWS)
                return WindowsCloseHandler.SetConsoleCtrlHandler(handler, add);
            else if (OS.CurrentOS == OperatingSystem.OSPlatform.UNIX)
            {
                AppDomain.CurrentDomain.ProcessExit += (s, e) => handler.Invoke(CtrlType.CTRL_CLOSE_EVENT); //I have no idea if that will work
            }
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
