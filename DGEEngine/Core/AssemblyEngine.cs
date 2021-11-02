using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core
{
    public static class AssemblyEngine
    {
        public const string NAME = "DGE-Engine";
        public const string VERSION = "0.1.0.2";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE");


        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {
            Paths.Add("ImageBuffers", Paths.Get("Storage") + "ImageBuffers/");
        }

    }
}
