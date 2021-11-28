using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core
{
    public static class AssemblyRemora
    {
        public const string NAME = "DGE-Remora";
        public const string VERSION = "0.0.0.0";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE-Remora");


        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {

        }

    }
}
