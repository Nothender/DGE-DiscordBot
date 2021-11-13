using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Core.OperatingSystem
{
    public interface IScript
    {

        public string FileName { get; }

        /// <summary>
        /// Defines an implementation for a target OS this implementation will be run only if on this specific OS
        /// </summary>
        /// <param name="platform">The target platform of this implementation</param>
        /// <param name="implementation">The code/scripting to be executed when Run is called</param>
        /// <returns>The script so you can chain the .DefineImplementation at construction</returns>
        public IScript DefineImplementation(OSPlatform platform, string implementation);

        /// <summary>
        /// Returns the implementation string for the given target platform, if there is no implementation for it, throws a NotImplementedException
        /// </summary>
        /// <param name="platform">The target platform on which this implementation is supposed to run</param>
        /// <returns>The implementation string</returns>
        public string GetImplementation(OSPlatform platform);

        /// <summary>
        /// Creates and saves the script to the disk, then executes it
        /// </summary>
        public void Run();
    }
}
