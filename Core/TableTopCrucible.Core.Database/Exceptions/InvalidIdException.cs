using System;

namespace TableTopCrucible.Core.Database.Exceptions
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException(string message) : base(message)
        {

        }
    }
}
