using System;

namespace TableTopCrucible.Core.Database.Exceptions
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
