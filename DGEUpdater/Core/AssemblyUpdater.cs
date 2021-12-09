using EnderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DGE.Core
{
    public static class AssemblyUpdater
    {
        public const string NAME = "DGE-Updater";
        public const string VERSION = "0.0.6.1";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE-Updater");


        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {

            Paths.Add("Updater", Paths.Get("Application") + "Updater/");
            Paths.Add("Downloads", Paths.Get("Updater") + "Downloads/");
            Paths.Add("Contents", Paths.Get("Updater") + "Contents/");

            //Paths.ClearPath("Downloads"); //Deleting it to delete every files in it and prevent bugs
            Paths.ClearPath("Contents");

        }

    }
}
