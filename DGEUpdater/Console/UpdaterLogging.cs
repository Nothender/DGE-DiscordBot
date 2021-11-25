using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Console
{
    public static class UpdaterLogging
    {

        public static void WriteToMain(string message, EnderEngine.Logger.LogLevel logLevel)
        {
            System.Console.WriteLine($"{Updater.UpdaterTags.GetLogTag(logLevel)}{message.Replace("\\n", "\n").Replace("\n", "\\n")}"); //Replace \\n by \n so we make sure there are only \n's and then replace all of them by \\n otherwise \\n would turn into \\\n
        }

    }
}
