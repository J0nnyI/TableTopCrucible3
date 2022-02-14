using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions;

public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException()
    {
    }

    public InvalidFileTypeException(string message) : base(message)
    {
    }
}