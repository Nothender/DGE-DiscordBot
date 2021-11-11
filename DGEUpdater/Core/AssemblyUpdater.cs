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
        public const string VERSION = "0.0.0.0";

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

            Directory.Delete(Paths.Get("Downloads"), true); //Deleting it to delete every files in it and prevent bugs
            Directory.CreateDirectory(Paths.Get("Downloads"));
            Directory.Delete(Paths.Get("Contents"), true);
            Directory.CreateDirectory(Paths.Get("Contents"));

        }

    }
}
