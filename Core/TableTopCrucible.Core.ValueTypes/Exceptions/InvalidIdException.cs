using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions;

public class InvalidIdException : Exception
{
    public InvalidIdException(string message) : base(message)
    {
    }
}