using System;

using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHashKey : ComplexValueType<FileHash, FileSize, FileHashKey>
    {
        public FileHash Hash
        {
            get => ValueA;
            init => ValueA = value;
        }
        public FileSize FileSize
        {
            get => ValueB;
            init => ValueB = value;
        }
        public static FileHashKey Create(FilePath file) => From(FileHash.Create(file), file.GetSize());
        public override string ToString() => FileSize + "_" + BitConverter.ToString(Hash.Value);
    }
}