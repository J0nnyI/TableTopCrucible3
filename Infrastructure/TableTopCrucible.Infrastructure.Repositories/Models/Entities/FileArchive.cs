using System;
using System.Diagnostics.CodeAnalysis;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class FileArchive : EntityBase<FileArchiveId>, IComparable<FileArchive>
    {
        [NotNull] public Name Name { get; init; }
        [NotNull] public FileArchivePath Path { get; init; }

        public FileArchive()
        {
            this.Id = FileArchiveId.New();
        }

        public int CompareTo(FileArchive other)
            => Name.CompareTo(other?.Name);
    }
}
