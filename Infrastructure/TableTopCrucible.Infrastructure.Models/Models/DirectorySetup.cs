 using System;
using System.Diagnostics.CodeAnalysis;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class DirectorySetup:IComparable<DirectorySetup>
    {
        [NotNull] public DirectorySetupId Id { get; init; }
        [NotNull] public Name Name { get; init; }
        [NotNull] public DirectorySetupPath Path { get; init; }


        public int CompareTo(DirectorySetup other)
            => Name.CompareTo(other?.Name);
        
        public DirectorySetup(vtName name, DirectorySetupPath path, DirectorySetupId Id = null)
        {
            Id = Id ?? DirectorySetupId.New();
            this.Name = name;
            this.Path = path;
        }

    }
}
