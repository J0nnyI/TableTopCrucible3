using System;
using TableTopCrucible.Infrastructure.Models.Entities;
using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHashKey : ComplexValueType<FileHash, FileSize, FileHashKey>
    {
        public FileHash Hash => ValueA;
        public FileSize FileSize => ValueB;

        public static FileHashKey Create(FilePath file) => From(FileHash.Create(file), file.GetSize());
    }
}