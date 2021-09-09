using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class InvalidFiletypeException : Exception
    {
        public InvalidFiletypeException() : base()
        {

        }
        public InvalidFiletypeException(string message) : base(message)
        {

        }
    }
}
