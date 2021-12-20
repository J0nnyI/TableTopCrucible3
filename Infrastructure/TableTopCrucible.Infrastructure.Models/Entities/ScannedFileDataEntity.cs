using System;
using System.Collections.Generic;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class ScannedFileDataEntity : DataEntity<ScannedFileDataId>
    {
        public ScannedFileDataEntity()
        {
        }

        public ScannedFileDataEntity(FilePath fileLocation, FileHashKey hashKey, DateTime lastWrite)
        {
            FileLocation = fileLocation;
            HashKey = hashKey;
            LastWrite = lastWrite;
        }

        public FilePath FileLocation { get; set; }
        public FileHashKey HashKey { get; set; }
        public DateTime LastWrite { get; set; }

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, FileLocation, HashKey, LastWrite };
    }
}