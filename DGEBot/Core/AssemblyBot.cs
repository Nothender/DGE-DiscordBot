using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core
{
    public static class AssemblyBot
    {

        /// <summary>
        /// The name of the Engine/Assembly
        /// </summary>
        public const string NAME = "DGEBot";
        /// <summary>
        /// The current version of the Engine in that format : Major.Minor.Fix/Small.Revision/SmallExtra
        /// </summary>
        public const string VERSION = "0.21.0.4";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE-Bot");

        private static bool assemblyInitialized = false;

        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        public static void Init()
        {
            if (assemblyInitialized)
            {
                logger.Log("DGE Bot is being initialized more than once", Logger.LogLevel.WARN);
                return;
            }
            assemblyInitialized = true;

            //Bot Init code

        }
    }
}