using System;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FileData : DataEntity<FileDataId>
    {
        private FileHashKey _hashKey;

        private DateTime _lastWrite;

        private FilePath _path;

        public FileData()
        {
        }

        public FileData(FilePath path, FileHashKey hashKey, DateTime lastWrite)
        {
            Path = path;
            HashKey = hashKey;
            LastWrite = lastWrite;
        }

        [JsonProperty]
        public FilePath Path
        {
            get => _path;
            set => SetRequiredValue(ref _path, value);
        }

        [JsonProperty]
        public FileHashKey HashKey
        {
            get => _hashKey;
            set => SetRequiredValue(ref _hashKey, value, nameof(HashKey));
        }

        [JsonProperty]
        public DateTime LastWrite
        {
            get => _lastWrite;
            set => SetRequiredValue(ref _lastWrite, value);
        }
    }
}