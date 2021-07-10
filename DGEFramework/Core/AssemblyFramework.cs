using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;

namespace DGE.Core
{
    public static class AssemblyFramework
    {

        public const string NAME = "DGE-Framework";
        public const string VERSION = "0.0.1.0";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE");


        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {
            //FW Init code
            Engine.Init();

        }

    }
}
