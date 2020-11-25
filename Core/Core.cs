using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DiscordGameEngine.Core
{
    public static class Core
    {

        public static readonly string pathToDataStorage = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static readonly string pathToAssets = pathToDataStorage + "\\Assets\\";
        public static readonly string pathToImageFrameBuffers = pathToDataStorage + "\\ImageBuffers\\";
        public static readonly string pathToSavedData = pathToDataStorage + "\\SavedData\\";

        internal static void Start()
        {
            CheckForDir(pathToDataStorage);
            CheckForDir(pathToAssets);
            CheckForDir(pathToImageFrameBuffers);
            CheckForDir(pathToSavedData);
        }

        /// <summary>
        /// Checks if the directory specified with dirPath exists, and if it doesn't creates it
        /// </summary>
        /// <param name="dirPath">Path to the directory</param>
        public static void CheckForDir(string dirPath)
        {
            if (Directory.Exists(dirPath))
                return;
            Directory.CreateDirectory(dirPath);
        }

    }
}
