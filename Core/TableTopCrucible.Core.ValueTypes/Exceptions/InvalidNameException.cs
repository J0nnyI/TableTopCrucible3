using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : base(message)
        {
        }
    }
}