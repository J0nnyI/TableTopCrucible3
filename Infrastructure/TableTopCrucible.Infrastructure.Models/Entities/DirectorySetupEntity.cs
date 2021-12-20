using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity : DataEntity<DirectorySetupId>
    {
        private Name _name;
        public Name Name
        {
            get => _name;
            set => RaiseAndSetRequiredIfChanged(ref _name, value);
        }

        private DirectoryPath _path;

        public DirectoryPath Path
        {
            get => _path;
            set => RaiseAndSetRequiredIfChanged(ref _path, value);
        }

        protected override IEnumerable<object> getAtomicValues() 
            => new object[]{Name, Path};

        public DirectorySetupEntity()
        {
                
        }

        public DirectorySetupEntity(DirectoryPath path)
        {
            this.Path = path;
            this.Name = path.GetDirectoryName().ToName();
        }
    }
}
