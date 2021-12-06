using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public class ScannedFileDataChangeSet
        :IDataChangeSet<ScannedFileDataId, ScannedFileDataModel, ScannedFileDataEntity>
    {
        public ScannedFileDataId Id { get; set; }
        public FileHashKey HashKey { get; set; }
        public FilePath FileLocation { get; set; }
        public DateTime LastWrite { get; set; }

        public ScannedFileDataChangeSet()
        {
            
        }
        
        public ScannedFileDataChangeSet(FileHashKey hashKey, FilePath fileLocation, DateTime lastWrite, ScannedFileDataId id = null)
        {
            Id = id ?? ScannedFileDataId.New();
            HashKey = hashKey;
            FileLocation = fileLocation;
            LastWrite = lastWrite;
        }

        public ScannedFileDataEntity ToEntity() 
            =>new()
            {
                FileLocation = FileLocation.Value,
                FileSize = HashKey.FileSize.Value,
                Hash = HashKey.Hash.Value,
                Id = Id.Guid,
                LastWrite = LastWrite
            };

        public ScannedFileDataModel ToModel()
            => new(HashKey, FileLocation, LastWrite, Id);
    }
}
