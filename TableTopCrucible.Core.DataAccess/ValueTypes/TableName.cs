using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.ValueTypes;

using ValueOf;

namespace TableTopCrucible.Core.FileManagement.ValueTypes
{
    public class TableName:ValueOf<string, TableName>
    {
        public static TableName FromType<Tid, Tentity>() where Tid:IEntityId where Tentity:IEntity<Tid>
        {
            return new TableName
            {
                Value = typeof(Tentity).AssemblyQualifiedName
            };
        }
        public RelativeDirectoryPath GetRelativePath()
            => RelativeDirectoryPath.From(this.Value);
    }
}
