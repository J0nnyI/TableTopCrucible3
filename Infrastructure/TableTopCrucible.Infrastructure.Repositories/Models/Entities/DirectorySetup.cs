using System;
using System.Diagnostics.CodeAnalysis;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class DirectorySetup : EntityBase<DirectorySetupId>, IComparable<DirectorySetup>
    {
        [NotNull] public Name Name { get; init; }
        [NotNull] public DirectorySetupPath Path { get; init; }


        public int CompareTo(DirectorySetup other)
            => Name.CompareTo(other?.Name);

        public DirectorySetup(string name, string path, DirectorySetupId Id = null) : this(vtName.From(name), DirectorySetupPath.From(path), Id)
        { }
        public DirectorySetup(vtName name, DirectorySetupPath path, DirectorySetupId Id = null) : base(Id ?? DirectorySetupId.New())
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
