using System;
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

        public static FileHashKey From(FileSize fileSize, FileHash hash)
            => From(fileSize.Value + "_" + BitConverter.ToString(hash.Value).Replace("-", ""));
    }
}