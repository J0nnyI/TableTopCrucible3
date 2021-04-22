using System;
using System.IO;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.ValueTypes.IDs;


namespace TableTopCrucible.Data.Models.Sources
{
    public class FileData
    {
        public FileData(FileInfo fileInfo, FileHash hash) : this(
            FilePath.From(fileInfo.FullName),
            FileDataHashKey.From((hash, FileSize.From(fileInfo.Length))),
            fileInfo.LastWriteTime)
        { }
        public FileData(FilePath path, FileDataHashKey hashKey, DateTime mostRecentUpdate)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            HashKey = hashKey ?? throw new ArgumentNullException(nameof(hashKey));
            MostRecentUpdate = mostRecentUpdate;
        }

        public FilePath Path { get; }
        public FileDataHashKey HashKey { get; }
        public DateTime MostRecentUpdate { get; set; }
        public FileType Type => this.Path.GetFileType();
    }
}
