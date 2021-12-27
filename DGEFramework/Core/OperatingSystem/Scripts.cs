using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core.OperatingSystem
{
    public static class Scripts
    {
        /// <summary>
        /// Runs the app, you have to specify the FULL FilePath to the application you want to run in the createprocess args
        /// </summary>
        public static readonly Script RunApp = new Script("RunApp")
            .DefineImplementation(OSPlatform.WINDOWS, "set app=%1\nstart %app%") as Script;
        /// <summary>
        /// Moves the contents of "Contents" folder, then restarts the app passed in arguments, NEEDS DGE to shutdown
        /// </summary>
        public static readonly Script UpdateRestartApp = new Script("UpdateRestartApp")
            .DefineImplementation(OSPlatform.WINDOWS, "set app=%1\nmove .\\Updater\\Contents\\*.* .\\\nstart %app%") as Script;
        /// <summary>
        /// Moves the contents of "Contents" folder, NEEDS DGE to shutdown
        /// </summary>
        public static readonly Script UpdateApp = new Script("UpdateApp")
            .DefineImplementation(OSPlatform.WINDOWS, "move .\\Updater\\Contents\\*.* .\\") as Script;
    }
}
