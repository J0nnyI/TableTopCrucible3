using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Domain.Models.ValueTypes;

using TableTopCurcible.Data.Library.Models.ValueTypes.General;
using TableTopCurcible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public class SourceDirectoryChangeset
        : ReactiveEntityBase<SourceDirectoryChangeset, SourceDirectory, SourceDirectoryId>,
        IEntityChangeset<SourceDirectory, SourceDirectoryId>
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public SourceDirectoryChangeset(SourceDirectory? origin = null) : base(origin)
        { }

        public override IEnumerable<Validator<SourceDirectoryChangeset>> Validators { get; }

        public override SourceDirectory Apply()
            => new SourceDirectory(Origin.Value, new Uri(Path), (DirectorySetupName)Name, (Description)Description);
        public override SourceDirectory ToEntity()
            => new SourceDirectory(new Uri(Path), (DirectorySetupName)Name, (Description)Description);


    }
}
