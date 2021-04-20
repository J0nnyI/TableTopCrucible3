
using System;
using System.IO;
using System.Security.Cryptography;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Data.Library.Models.ValueTypes.Exceptions;

using ValueOf;

namespace TableTopCrucible.Data.Library.Models.ValueTypes.General
{
    public class FileHash : ValueOf<byte[], FileHash>
    {
        protected override void Validate()
        {
            if (Value == null)
                throw new NullReferenceException("The hash must not be empty");
            if (Value.Length != HashHelper.SHA512_Size)
                throw new InvalidHashSizeException(Value.Length);
        }
        public static FileHash Create(FilePath path, HashAlgorithm hashAlgorithm)
        {
            using FileStream stream = File.OpenRead(path);
            byte[] data = hashAlgorithm.ComputeHash(stream);
            return From(data);
        }
        public static FileHash Create(FilePath path)
        {
            using var hashAlgorithm = SHA512.Create();
            return Create(path, hashAlgorithm);
        }

    }
}