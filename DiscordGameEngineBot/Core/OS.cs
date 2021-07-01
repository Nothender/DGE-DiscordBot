using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordGameEngine.Core
{

    public enum OSPlatform
    {
        WINDOWS,
        UNIX,
        MACOS,
        UNKNOWN
    }

    public static class OS
    {

        public static readonly OSPlatform CurrentOS = GetRunningOS();

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

    }
}
