using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.DataAccess.Exceptions
{
    public class SaveFailedException : Exception
    {
        public SaveFailedException()
        {

        }
        public SaveFailedException(Exception innerException) : base(null, innerException) { }
        public SaveFailedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
