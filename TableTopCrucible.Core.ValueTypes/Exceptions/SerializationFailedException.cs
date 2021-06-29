using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class SerializationFailedException : Exception
    {
        public SerializationFailedException(Exception innerException):base(null, innerException)
        {

        }
    }
}
