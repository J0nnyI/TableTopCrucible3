using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions;

public class InvalidValueException : Exception
{
    public InvalidValueException(string message) : base(message)
    {
    }
}