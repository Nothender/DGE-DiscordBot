using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static DGE.Core.CloseEvent;

namespace DGE.Core
{
    /// <summary>
    /// DO NOT LOAD IF ON ANY OTHER PLATFORM THAN WINDOWS -> CRASHES APPLICATION
    /// </summary>
    internal static class WindowsCloseHandler
    {
        [DllImport("Kernel32")]
        internal static extern bool SetConsoleCtrlHandler(CtrlEventHandler handler, bool add);
    }
}
