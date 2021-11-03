using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Exceptions
{
    public class InvalidArgumentCountException : Exception
    {
        public InvalidArgumentCountException(string commandName, int required, int found) :
            base(found > required ? $"Too many arguments for the `{commandName}` command (given : {found}, expected : {required})" : $"Too few arguments for the `{commandName}` command ({found} instead of {required})")
        { }

    }
}
