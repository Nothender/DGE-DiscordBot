using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using EnderEngine;

namespace DGE.Core
{
    public static class AssemblyFramework
    {

        public const string NAME = "DGE-Framework";
        public const string VERSION = "0.1.8.3";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger { get; } = new Logger("DGE");

        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {
            //FW Init code
            Engine.Init();
            Paths.Add("SaveData", Paths.Get("Storage") + "SaveData/");
            Paths.Add("Assets", Paths.Get("Storage") + "Assets/");
            Paths.Add("Scripts", Paths.Get("Storage") + "Scripts/");
        }

    }
}
