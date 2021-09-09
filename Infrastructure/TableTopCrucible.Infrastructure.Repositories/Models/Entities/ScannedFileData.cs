using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class ScannedFileData : EntityBase<ScannedFileDataId>
    {
        public FileHashKey HashKey { get; }
        public FilePath FileLocation { get; }
        public DateTime LastWrite { get; }

        public ScannedFileData(FileHashKey hashKey, FilePath fileLocation, DateTime lastWrite, ScannedFileDataId id = null) : base(id ?? ScannedFileDataId.New())
        {
            HashKey = hashKey;
            FileLocation = fileLocation;
            LastWrite = lastWrite;
        }
    }
}
