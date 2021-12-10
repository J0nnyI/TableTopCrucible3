using System;
using System.Runtime.Serialization;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ScannedFileDataEntity : IDataEntity<ScannedFileDataId>
    {
        public Guid Id { get; set; }
        public string FileLocation { get; set; }
        public byte[] Hash { get; set; }
        public long FileSize { get; set; }
        public DateTime LastWrite { get; set; }
    }
}
