using System;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Models
{
    public class ScannedFileDataModel : IDataModel<ScannedFileDataId>
    {
        public ScannedFileDataId Id { get; }
        public FileHashKey HashKey { get; }
        public FilePath FileLocation { get; }
        public DateTime LastWrite { get; }

        public ScannedFileDataModel(FileHashKey hashKey, FilePath fileLocation, DateTime lastWrite, ScannedFileDataId id = null)
        {
            Id = id ?? ScannedFileDataId.New();
            HashKey = hashKey;
            FileLocation = fileLocation;
            LastWrite = lastWrite;
        }
    }
}
