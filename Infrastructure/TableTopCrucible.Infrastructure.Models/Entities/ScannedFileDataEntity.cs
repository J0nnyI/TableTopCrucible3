using System;
using System.Runtime.Serialization;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ScannedFileDataEntity:IDataEntity
    {
        public Guid Id { get; set; }
        public string FileLocation { get; set; }
        public byte[] Hash { get; set; }
        public long FileSize { get; set; }
        public DateTime LastWrite { get; set; }

        public ScannedFileDataModel ToEntity()
        {
            return new (
                FileHashKey.From(
                    FileHash.From(Hash),
                    Core.ValueTypes.FileSize.From(FileSize)
                ),
                (FilePath) FileLocation,
                LastWrite,
                (ScannedFileDataId) Id
            );
        }

        public void Initialize(ScannedFileDataModel sourceEntity)
        {
            Id = sourceEntity.Id.Value;
            FileLocation = sourceEntity.FileLocation.Value;
            Hash = sourceEntity.HashKey.Hash.Value;
            FileSize = sourceEntity.HashKey.FileSize.Value;
            LastWrite = sourceEntity.LastWrite;
        }

    }
}
