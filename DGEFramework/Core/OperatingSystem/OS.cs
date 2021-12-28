using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DGE.Core.OperatingSystem
{
    /// <summary>
    /// Enum to identify the os/platform
    /// </summary>
    public enum OSPlatform
    {
        WINDOWS = 0,
        UNIX,
        MACOS,
        UNKNOWN
    }

    public static class OS
    {
        /// <summary>
        /// The OS the app is currently running on
        /// </summary>
        public static readonly OSPlatform CurrentOS = GetRunningOS();

        /// <summary>
        /// Gets the OS on which the app is running
        /// </summary>
        /// <returns>The current OS on which the app is running</returns>
        private static OSPlatform GetRunningOS()
        {
            if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return OSPlatform.WINDOWS;
            else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return OSPlatform.UNIX;
            else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                return OSPlatform.MACOS;
            return OSPlatform.UNKNOWN;
        }

        #region Utils
        private static string[] dotnetExtensions = new string[4]
        {
            ".exe",
            ".dll",
            ".dll",
            ".dll"
        };

        /// <summary>
        /// Gets the extension to run a .NET app depending on the current OS
        /// </summary>
        /// <returns> Returns a string from 2 to 3 characters containing either ".exe" or ".dll" </returns>
        public static string GetDotnetExtension() => dotnetExtensions[(int)OS.CurrentOS];

        #endregion Utils

    }

}
