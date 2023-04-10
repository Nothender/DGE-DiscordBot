using DGE.Console;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Core
{

    public static class AssemblyNothenderBot
    {

        /// <summary>
        /// The name of the Engine/Assembly
        /// </summary>
        public const string NAME = "DGE-NothenderBot";
        /// <summary>
        /// The current version of the Engine in that format : Major.Minor.Fix/Small.Revision/SmallExtra
        /// </summary>
        public const string VERSION = "1.0.0.0";

        public static readonly DGEModule module = new DGEModule(NAME, VERSION, Init);

        internal static Logger logger = new Logger("DGE-NothenderBot");

        /// <summary>
        /// Init function for the assembly (DGEModule)
        /// </summary>
        private static void Init()
        {

        }
    }
}
