
using System;
using System.IO.Abstractions;

using TableTopCrucible.Core.ValueTypes;


namespace TableTopCrucible.Data.Models.Sources
{
    public class FileData
    {
        public FileData(IFileInfo fileInfo, FileHash hash) : this(
            FilePath.From(fileInfo.FullName),
            FileHashKey.From((hash, FileSize.From(fileInfo.Length))),
            fileInfo.LastWriteTime)
        { }
        public FileData(FilePath path, FileHashKey hashKey, DateTime mostRecentUpdate) : this()
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Hash = hashKey.Hash;
            Size = hashKey.FileSize;
            MostRecentUpdate = mostRecentUpdate;
        }
        /// <summary>
        /// should be used for automapper only
        /// </summary>
        public FileData()
        {
        }

        public FilePath Path { get; private set; }
        public FileHash Hash { get; private set; }
        public FileSize Size { get; private set; }
        public FileHashKey HashKey => FileHashKey.From(Hash, Size);

        public DateTime MostRecentUpdate { get; private set; }

        public FileType Type => this.Path?.GetFileType() ?? FileType.Other;


        public override int GetHashCode()
        {
            return HashCode.Combine(Path, Hash, Size, MostRecentUpdate, Type);
        }

        public override bool Equals(object obj)
        {
            return obj is FileData data &&
                   Path == data.Path &&
                   Hash == data.Hash &&
                   Size == data.Size &&
                   MostRecentUpdate == data.MostRecentUpdate &&
                   Type == data.Type;
        }
    }
}
