using System;
using System.Diagnostics.CodeAnalysis;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.ValueTypes;
using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Infrastructure.Models.Models
{

    public class DirectorySetupModel :IDataModel<DirectorySetupId>, IComparable<DirectorySetupModel>
    {
        [NotNull] public DirectorySetupId Id { get; init; }
        [NotNull] public Name Name { get; init; }
        [NotNull] public DirectorySetupPath Path { get; init; }


        public int CompareTo(DirectorySetupModel other)
            => Name.CompareTo(other?.Name);

        public DirectorySetupModel(vtName name, DirectorySetupPath path, DirectorySetupId id = null)
        {
            Id = id ?? DirectorySetupId.New();
            this.Name = name;
            this.Path = path;
        }

    }
}
