using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core.OperatingSystem
{
    public static class Scripts
    {
        //TODO: Will contain basic scripts, like reboot, run, update, etc...
        /// <summary>
        /// Runs the app, you have to specify the FULL FilePath to the application you want to run in the createprocess args
        /// </summary>
        public static readonly Script RunApp = new Script("RunApp").DefineImplementation(OSPlatform.WINDOWS, "set app=%1\nstart %app%") as Script;
    }
}
