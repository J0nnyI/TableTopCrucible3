using System;


namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class InvalidHashSizeException : Exception
    {
        public InvalidHashSizeException(int actualLength) : base($"the Hash has a size of {actualLength} bytes but is required to be {FileHash.SHA512_Size} bytes long.")
        {
        }

    }
}
