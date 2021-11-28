using System;
using System.Runtime.Serialization;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    [DataContract]
    public class ScannedFileDataDto : IEntityDto<ScannedFileDataId, ScannedFileData>
    {
        public Guid Id { get; set; }
        public string FileLocation { get; set; }
        public byte[] Hash { get; set; }
        public long FileSize { get; set; }
        public DateTime LastWrite { get; set; }

        public ScannedFileData ToEntity()
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

        public void Initialize(ScannedFileData sourceEntity)
        {
            Id = sourceEntity.Id.Value;
            FileLocation = sourceEntity.FileLocation.Value;
            Hash = sourceEntity.HashKey.Hash.Value;
            FileSize = sourceEntity.HashKey.FileSize.Value;
            LastWrite = sourceEntity.LastWrite;
        }

    }
}
