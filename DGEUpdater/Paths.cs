using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DGE
{
    public static class Paths
    {
        public static readonly string ProgramPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace('\\', '/') + "/";
        public static readonly string AUPath = ProgramPath + "Updater/";
        public static readonly string DownloadPath = AUPath + "Downloads/";
    }
}
