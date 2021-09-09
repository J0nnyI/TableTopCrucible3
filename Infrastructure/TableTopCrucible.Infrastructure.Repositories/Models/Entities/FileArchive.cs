using System;
using System.Diagnostics.CodeAnalysis;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class FileArchive : EntityBase<FileArchiveId>, IComparable<FileArchive>
    {
        [NotNull] public Name Name { get; init; }
        [NotNull] public FileArchivePath Path { get; init; }


        public int CompareTo(FileArchive other)
            => Name.CompareTo(other?.Name);

        public FileArchive(string name, string path, FileArchiveId Id = null):this(vtName.From(name), FileArchivePath.From(path), Id)
        { }
        public FileArchive(vtName name, FileArchivePath path, FileArchiveId Id = null)
        {
            this.Id = Id ?? FileArchiveId.New();
            this.Name = name;
            this.Path = path;
        }
    }
}
