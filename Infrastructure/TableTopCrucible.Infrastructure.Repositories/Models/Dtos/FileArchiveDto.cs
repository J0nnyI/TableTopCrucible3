﻿using System;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class FileArchiveDto : IEntityDto<FileArchiveId, FileArchive>
    {
        public string Name { get; }
        public string Path { get; }
        public DateTime LastSync { get; }
        public Guid IdValue { get; set; }
    }
}
