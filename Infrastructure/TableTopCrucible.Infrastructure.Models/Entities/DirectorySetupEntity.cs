using System.Collections.Generic;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity : DataEntity<DirectorySetupId>
    {
        private Name _name;

        private DirectoryPath _path;

        public DirectorySetupEntity()
        {
        }

        public DirectorySetupEntity(DirectoryPath path)
        {
            Path = path;
            Name = path.GetDirectoryName().ToName();
        }

        public Name Name
        {
            get => _name;
            set => RaiseAndSetRequiredIfChanged(ref _name, value);
        }

        public DirectoryPath Path
        {
            get => _path;
            set => RaiseAndSetRequiredIfChanged(ref _path, value);
        }

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Name, Path };
    }
}