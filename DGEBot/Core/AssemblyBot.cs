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
        public const string NAME = "DGE-Bot";
        /// <summary>
        /// The current version of the Engine in that format : Major.Minor.Fix/Small.Revision/SmallExtra
        /// </summary>
        public const string VERSION = "0.21.10.7";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE-Bot");

        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {

            //Bot Init code

        }
    }
}