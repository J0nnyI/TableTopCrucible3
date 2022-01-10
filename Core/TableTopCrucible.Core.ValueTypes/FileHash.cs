using System;
using System.Linq;
using System.Security.Cryptography;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHash : ValueType<byte[], FileHash>
    {
        public static int SHA512_Size = 64;

        protected override void Validate(byte[] value)
        {
            if (value == null)
                throw new NullReferenceException("The hash must not be empty");
            if (value.Length != SHA512_Size)
                throw new InvalidHashSizeException(Value.Length);
        }

        public static FileHash Create(FilePath filePath, HashAlgorithm hashAlgorithm)
        {
            using var stream = filePath.OpenRead();
            var data = hashAlgorithm.ComputeHash(stream);
            return From(data);
        }

        public static FileHash Create(FilePath path)
        {
            using var hashAlgorithm = SHA512.Create();
            return Create(path, hashAlgorithm);
        }

        public override bool Equals(object obj) =>
            obj is FileHash hash &&
            Value.SequenceEqual(hash.Value);
        

        public override int GetHashCode()
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            =>
                HashCode.Combine(base.GetHashCode(), Value);

        public override string ToString() => BitConverter.ToString(Value, 0);
    }
}