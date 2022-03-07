using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper
{
    public static class HashHelper
    {
        public static HashAlgorithm CreateHashAlgorithm()
            => HashAlgorithm.Create(HashAlgorithmName.SHA512.ToString());
    }
}
