using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Exceptions
{
    public class InvalidArgumentTypeException : Exception
    {
        public InvalidArgumentTypeException(int argumentPosition, Type mustBe) :
               base($"argument {argumentPosition} should be of type {mustBe.Name}")
        { }
    }
}
