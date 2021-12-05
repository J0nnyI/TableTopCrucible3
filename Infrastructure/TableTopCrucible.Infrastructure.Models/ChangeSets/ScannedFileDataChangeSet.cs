using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public class ScannedFileDataChangeSet
    {
        public ScannedFileDataId Id { get; }
        public FileHashKey HashKey { get; }
        public FilePath FileLocation { get; }
        public DateTime LastWrite { get; }

    }
}
