using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;

namespace DGE.Core
{
    public static class AssemblyFramework
    {

        public const string NAME = "DGEFramework";
        public const string VERSION = "0.0.0.0";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE");

        private static bool assemblyInitialized = false;

        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        public static void Init()
        {
            if (assemblyInitialized)
            {
                logger.Log("DGE Framework is being initialized more than once", Logger.LogLevel.WARN);
                return;
            }
            assemblyInitialized = true;

            //FW Init code
            Engine.Init();

        }

    }
}
