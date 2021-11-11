using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;
using System.Diagnostics;

namespace DGE
{
    public static class ScriptsManager
    {

        /// <summary>
        /// Creates update scripts that shutdown the running applications, and moves the files from contents to the current application folder, then restarts the main application
        /// </summary>
        public static void RunScript(string launchPId)
        {
            string upid = Process.GetCurrentProcess().Id.ToString();
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo
            {
                FileName = Paths.Get("Application") + "WindowsUScript.bat",
                Arguments = $"{upid} {launchPId}"
            };
            p.Start();
        }

    }
}
