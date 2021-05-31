
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using TableTopCrucible.Core.ValueTypes.Exceptions;
using static TableTopCrucible.Core.BaseUtils.FileSystemHelper;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHash : ValueOf<byte[], FileHash>
    {
        public static int SHA512_Size = 64;
        protected override void Validate()
        {
            if (Value == null)
                throw new NullReferenceException("The hash must not be empty");
            if (Value.Length != SHA512_Size)
                throw new InvalidHashSizeException(Value.Length);
        }
        public static FileHash Create(FilePath filePath, HashAlgorithm hashAlgorithm)
        {
            using var stream = filePath.OpenRead();
            byte[] data = hashAlgorithm.ComputeHash(stream);
            return From(data);
        }
        public static FileHash Create(FilePath path)
        {
            using var hashAlgorithm = SHA512.Create();
            return Create(path, hashAlgorithm);
        }

        public override bool Equals(object obj)
            => obj is FileHash hash &&
                this.Value.SequenceEqual(hash.Value);
        protected override bool Equals(ValueOf<byte[], FileHash> other)
            => Equals(other as object);
        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Value);

    }
}