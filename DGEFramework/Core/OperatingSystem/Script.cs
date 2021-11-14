using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DGE.Core.OperatingSystem
{
    public class Script : IScript
    {

        public static readonly string[] Extensions = new string[4] 
        {
            "bat",
            "sh",
            "sh",
            "sh" //4th OS undefined, defaulting to unix
        };

        private readonly string[] implementations = new string[3]; //3 OS (the 4th one is unknown)

        public string FileName { get; }

        private Process process;

        /// <summary>
        /// Constructs the script, with the associated name, use DefineImplementation to create an implementation for an OS
        /// </summary>
        /// <param name="fileName">The name of the file it will be saved as to be executed (so be careful what you name it)</param>
        public Script(string fileName) => FileName = fileName.Trim().Replace(" ", "_");

        public IScript DefineImplementation(OSPlatform platform, string implementation)
        {
            if (platform == OSPlatform.UNKNOWN) platform = OSPlatform.UNIX; // Default to unix, bc most of the time if it is unknown it is unix based
            implementations[(int)platform] = implementation;
            return this;
        }

        public string GetImplementation(OSPlatform platform)
        {
            if (platform == OSPlatform.UNKNOWN) platform = OSPlatform.UNIX; // Default to unix, bc most of the time if it is unknown it is unix based
            return implementations[(int)platform] ?? throw new NotImplementedException($"There are no implementation for the specified OS {platform}");
        }

        /// <summary>
        /// Creates the process for execution but doesn't start it (Ability to start it using Run)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Process CreateProcess(params string[] args)
        {
            string filePath = Paths.Get("Scripts") + $"{FileName}.{Extensions[(int) OS.CurrentOS]}";
            string sArgs = string.Join(' ', args);
            File.WriteAllText(filePath, GetImplementation(OS.CurrentOS));
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo
            {
                FileName = filePath,
                Arguments = sArgs
            };
            process?.Dispose();
            process = p;
            return p;
        }

        /// <summary>
        /// Starts the script by using the process if created, if not creates one with no arguments
        /// </summary>
        public void Run()
        {
            if (process is null)
                CreateProcess();

            process.Start();

            //TODO: Log output using Framework logger, or create an OS/Scripts logger

        }
    }
}
