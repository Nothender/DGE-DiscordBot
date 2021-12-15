using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DGE;

namespace DGE.Core
{
    /// <summary>
    /// Handles correct closing of the application
    /// </summary>
    public static class CloseEvent
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(CtrlEventHandler handler, bool add);

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
