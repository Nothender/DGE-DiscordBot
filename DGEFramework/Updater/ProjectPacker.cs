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
        /// List of files that will be included in the packing (*.extension writing is allowed), Including folders may not work
        /// </summary>
        public static List<string> includes = new List<string>()
        {
            "*.dll",                    // Dependencies
            "*.exe",                    // Executables
            "*.deps.json",              // Configs for runtime (specifies dependencies)
            "*.runtimeconfig.json",     // Configs for runtime (specifies dependencies)
            "ProjectInfoConfig.xml",    // Updating info XML file for updater
            "*.dll",                    // Dependencies
            "*.pdb"                     // Debug files
        };

        /// <summary>
        /// List of files that will be included in the packing (If full setting applied when packign) (*.extension writing is allowed), Including folders may not work
        /// </summary>
        public static List<string> fullIncludes = new List<string>()
        {
            "config.txt",
            "config-exp.txt"
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

            foreach(string file in filesToInclude)
            {
                string temp_file = Path.Combine(temp_dir, Path.GetFileName(file));
                File.Copy(file, temp_file);
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

            foreach(string file in Directory.EnumerateFiles(Paths.Get("Application")))
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

            return filesPath.ToArray();
        }

    }
}
