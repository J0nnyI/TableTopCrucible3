using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class ScannedFileDataDto:IEntityDto<ScannedFileDataId, ScannedFileData>
    {
        public Guid IdValue { get; set; }
        public string FileLocationValue { get; set; }
    }
}
