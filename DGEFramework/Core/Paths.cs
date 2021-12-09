using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DGE.Core
{
    public static class Paths
    {

        private static Dictionary<string, string> paths = new Dictionary<string, string>();

        static Paths()
        {
            Add("Application", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace('\\', '/') + '/'); //Running application location
            Add("Storage", Get("Application") + "/Storage/"); //Default
        }

        public static string Get(string name)
        {
            if (name is null || paths.ContainsKey(name))
                return Get("Storage");
            return paths[name.ToLower()];
        }

        /// <summary>
        /// Adds a global path, and can be accessed with Paths.Get("name"). If it is a directory it is automatically created if it doesn't exist
        /// </summary>
        /// <param name="name">The name of the path you want to save it with</param>
        /// <param name="path">The path to the file or directory, if it is a directory please specify using the character '/' at the end of the string</param>
        public static void Add(string name, string path)
        {
            name = name.ToLower();
            if (paths.ContainsKey(name))
            {
                AssemblyFramework.logger.Log($"Couldn't add the path [{name}] \"{path}\" because a path with that name already exists", EnderEngine.Logger.LogLevel.ERROR);
                return;
            }
            path = path.Replace('\\', '/');
            paths.Add(name, path);
            if (path.EndsWith('/'))
                CheckForDir(path);
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

        /// <summary>
        /// Deletes all the files from a directory
        /// </summary>
        /// <param name="name"> The key to access the path </param>
        public static void ClearPath(string name)
        {
            string path = Get(name);
            if (Directory.Exists(path)) // If the path leads to a directory, and it exists
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }

    }
}
