using System;
using System.Linq;
using System.Security.Cryptography;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.ValueTypes;

public class FileHashKey : ValueType<string, FileHashKey>
{
    public static FileHashKey Create<TFilePath>(FilePath<TFilePath> file, HashAlgorithm hashAlgorithm = null)
        where TFilePath : FilePath<TFilePath>, new()
    {
        var useHash = hashAlgorithm ?? HashHelper.CreateHashAlgorithm();

        var res = From(file.GetSize(), FileHash.Create(file, useHash));

        if (hashAlgorithm == null)
            useHash.Dispose();

        return res;
    }

    private const string Separator = "_";

    public static FileHashKey From(FileSize fileSize, FileHash hash)
        => From(fileSize.Value + Separator + BitConverter.ToString(hash.Value).Replace("-", ""));

    public FileSize GetFileSizeComponent()
        => (FileSize)int.Parse(Value.Split(Separator).First());
}