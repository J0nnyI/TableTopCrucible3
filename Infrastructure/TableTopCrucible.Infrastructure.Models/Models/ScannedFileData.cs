using System;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class ScannedFileData
    {
        public ScannedFileDataId Id { get; }
        public FileHashKey HashKey { get; }
        public FilePath FileLocation { get; }
        public DateTime LastWrite { get; }

        public ScannedFileData(FileHashKey hashKey, FilePath fileLocation, DateTime lastWrite, ScannedFileDataId id = null)
        {
            Id = id ?? ScannedFileDataId.New();
            HashKey = hashKey;
            FileLocation = fileLocation;
            LastWrite = lastWrite;
        }
    }
}
