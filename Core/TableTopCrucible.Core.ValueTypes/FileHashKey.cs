using System;
using System.Linq;
using System.Security.Cryptography;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHashKey : ValueType<string, FileHashKey>
    {
        public static FileHashKey Create<TFilePath>(FilePath<TFilePath> file, HashAlgorithm hashAlgorithm = null)
            where TFilePath : FilePath<TFilePath>, new()
        {
            var useHash = hashAlgorithm ?? new SHA512Managed();

            var res = From(file.GetSize(), FileHash.Create(file, useHash));

            if (hashAlgorithm == null)
                useHash.Dispose();

            return res;
        }

        private static readonly string _separator = "_";
        public static FileHashKey From(FileSize fileSize, FileHash hash)
            => From(fileSize.Value + _separator + BitConverter.ToString(hash.Value).Replace("-", ""));

        public FileSize GetFileSizeComponent()
            => (FileSize)int.Parse(Value.Split(_separator).First());
    }
}