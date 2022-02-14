using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions;

public class SerializationFailedException : Exception
{
    public SerializationFailedException(Exception innerException) : base(null, innerException)
    {
    }
}