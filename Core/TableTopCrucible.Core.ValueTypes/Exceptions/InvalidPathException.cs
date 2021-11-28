using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string message = null, Exception innerException = null) : base(message,
            innerException)
        {
        }
    }
}