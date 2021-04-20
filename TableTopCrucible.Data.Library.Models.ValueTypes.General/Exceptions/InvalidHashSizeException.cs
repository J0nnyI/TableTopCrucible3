using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Data.Library.Models.ValueTypes.Exceptions
{
    public class InvalidHashSizeException:Exception
    {
        public InvalidHashSizeException(int actualLength):base($"the Hash has a size of {actualLength} bytes but is required to be {HashHelper.SHA512_Size} bytes long.")
        {
        }

    }
}
