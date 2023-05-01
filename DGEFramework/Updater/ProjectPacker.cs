using DGE.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DGE.Updater
{
    public static class ProjectPacker
    {


        /// <summary>
        /// List of files that will be included in the packing (*.extension writing is allowed), Include folders by adding a '/' at the end or surrounding them with it
        /// </summary>
        public static List<string> includes = new List<string>()
        {
            "*.dll",                    // Dependencies
            "*.exe",                    // Executables
            "*.deps.json",              // Configs for runtime (specifies dependencies)
            "*.runtimeconfig.json",     // Configs for runtime (specifies dependencies)
            "ProjectInfoConfig.xml",    // Updating info XML file for updater
            "*.dll",                    // Dependencies
            "*.pdb",                    // Debug files
            "runtimes/"                 // runtime dependencies for different OS
        };

        /// <summary>
        /// List of files that will be included in the packing (If full setting applied when packign) (*.extension writing is allowed), Including folders may not work
        /// </summary>
        public static List<string> fullIncludes = new List<string>()
        {
            "*Config.xml",
            "*ConfigDebug.xml"
        };

        /// <summary>
        /// Creates a .zip file containing all the important files for release (application.exe)
        /// </summary>
        /// <param name="fileName"> The name with which the file will be packed </param>
        /// <param name="full"> If true, configs and settings are packed </param>
        /// <returns> The FilePath to the .zip </returns>
        public static string Pack(string fileName, bool full)
        {
            // Remark : does not yet pack a config example file - see configuration handling (if not 'full')

            string[] filesToInclude = SelectFiles(full);

            string temp_dir = Paths.Get("Application") + "Temp/" + $"TPD-{fileName}/";
            string output_path = Paths.Get("Application") + $"{fileName}.zip";
            
            if (Directory.Exists(temp_dir))
                Directory.Delete(temp_dir, true);

            Directory.CreateDirectory(temp_dir);

            foreach (string file in filesToInclude)
            {
                if (file.EndsWith('/')) // File is folder
                {
                    string temp_folder = Path.Combine(temp_dir, Path.GetFileName(Path.GetDirectoryName(file)));
                    Paths.CopyDirectory(file.TrimEnd('/'), temp_folder.TrimEnd('/'), true);
                }
                else
                {
                    string temp_file = Path.Combine(temp_dir, Path.GetFileName(file));
                    File.Copy(file, temp_file);
                }
            }

            if (File.Exists(output_path)) File.Delete(output_path); // Overriding if exists
            ZipFile.CreateFromDirectory(temp_dir, output_path);

            Directory.Delete(temp_dir, true); // Cleaning

            return output_path;
        }

        /// <summary>
        /// Selects the include files in the application path
        /// </summary>
        /// <returns> The FullName of selected files </returns>
        private static string[] SelectFiles(bool fullPack)
        {
            List<string> filesPath = new List<string>();

            string[] includingFiles = new string[fullPack ? includes.Count + fullIncludes.Count : includes.Count];
            includes.CopyTo(includingFiles, 0); // Adding include files
            if (fullPack) fullIncludes.CopyTo(includingFiles, includes.Count); // If full packing, adding configs and settings
            // Code could be cleaner but who cares
            foreach(string file in Directory.EnumerateFiles(Paths.Get("Application"))) // Files
            {
                foreach(string include in includingFiles)
                {
                    if ((file.EndsWith(include)) || (include.StartsWith('*') && file.EndsWith(include.TrimStart('*')))) // Implement cleaner algorithm later (Actually its ok if i leave it like that)
                    {
                        filesPath.Add(file);
                        break;
                    }
                }
            }
            foreach (string folder in Directory.GetDirectories(Paths.Get("Application"))) // Folders
            {
                foreach (string include in includingFiles)
                {
                    if (!include.EndsWith('/')) continue;
                    string includeFolder = include.Trim('/');
                    if (folder.EndsWith(includeFolder) || (includeFolder.StartsWith('*') && folder.EndsWith(include.TrimStart('*')))) // Implement cleaner algorithm later (Actually its ok if i leave it like that)
                    {
                        filesPath.Add(folder + '/');
                        break;
                    }
                }
            }
            return filesPath.ToArray();
        }

    }
}
