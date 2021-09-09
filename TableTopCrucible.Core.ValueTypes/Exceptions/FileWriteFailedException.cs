using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class FileWriteFailedException:Exception
    {
        public FileWriteFailedException(Exception innerException):base(null, innerException)
        {

        }
    }
}
