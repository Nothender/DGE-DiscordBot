using System;
using System.Collections.Generic;
using System.Text;

namespace DGE
{
    public interface IVersion
    {
        
        public string version { get; set; }

        public bool IsNewer(IVersion other);

    }
}
