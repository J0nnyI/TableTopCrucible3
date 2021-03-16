using System;

namespace TableTopCrucible.Data.Library.Models.ValueTypes
{
    public struct FileDataHashKey
    {
        public FileHash FileHash { get; }
        public long FileSize { get; }
        /// <summary>
        /// shoould be only used when there is not fileInfo
        /// </summary>
        /// <param name="fileHash"></param>
        /// <param name="fileSize"></param>
        public FileDataHashKey(FileHash fileHash, long fileSize)
        {
            FileHash = fileHash;
            FileSize = fileSize;
        }

        public override string ToString() => $"{FileHash} : ${FileSize}";
        public override bool Equals(object obj)
        => obj is FileDataHashKey key && FileHash == key.FileHash && FileSize == key.FileSize;
        public override int GetHashCode() => this == default ? 0 : HashCode.Combine(FileHash, FileSize);

        public static bool operator ==(FileDataHashKey key1, FileDataHashKey key2)
            => key1.Equals(key2);
        public static bool operator !=(FileDataHashKey key1, FileDataHashKey key2)
            => !key1.Equals(key2);
    }

}
