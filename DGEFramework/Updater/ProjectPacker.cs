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
        /// Creates a .zip file containing all the important files for release (application.exe)
        /// </summary>
        public static void Pack(string fileName)
        {
            // Remark : does not yet pack a config example file - see configuration handling

            string[] filesToInclude = SelectFiles();

            Directory.CreateDirectory(Paths.Get("Application") + "Temp/" + $"TPD-{fileName}/");

            Directory.Delete(Paths.Get("Application") + "Temp/" + $"TPD-{fileName}/");

        }

        /// <summary>
        /// Selects the include files in the application path
        /// </summary>
        /// <returns> The FullName of selected files </returns>
        private static string[] SelectFiles()
        {
            List<string> filesPath = new List<string>();

            foreach(string file in Directory.EnumerateFiles(Paths.Get("Application")))
            {
                foreach(string include in includes)
                {
                    if ((include == file) || (include.StartsWith('*') && file.EndsWith(include.TrimStart('*')))) // Implement cleaner algorithm later (Actually its ok if i leave it like that)
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
