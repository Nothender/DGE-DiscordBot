using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DiscordGameEngine.Core
{
    public static class Core
    {

        public static readonly string PathToStorage = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Storage\\";
        public static readonly string pathToAssets = PathToStorage + "Assets\\";
        public static readonly string pathToImageFrameBuffers = PathToStorage + "ImageBuffers\\";
        public static readonly string pathToSavedData = PathToStorage + "SavedData\\";

        internal static void Start()
        {
            CheckForDir(PathToStorage);
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
