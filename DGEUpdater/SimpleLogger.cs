using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DGE
{
    public static class SimpleLogger
    {

        public static string filePath;

        public static void Init(string directory)
        {
            filePath = $"{directory}{(directory.EndsWith("/") ? "" : "/")}SimpleLog_{DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-')}.log";
            Console.WriteLine($"SimpleLogger initialized, LogFile Location: {filePath}");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.Create(filePath).Close();
            
        }

        public static void Log(string message, int logLevel = 0)
        {
            string s = $"[SL] [AutoUpdater] [{logLevels[logLevel % logLevels.Length]}]: {message}";
            Console.WriteLine(s);
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(s);
            }
                
        }

        private static string[] logLevels = { "Info", "Debug", "Error", "FATAL" };

    }
}
