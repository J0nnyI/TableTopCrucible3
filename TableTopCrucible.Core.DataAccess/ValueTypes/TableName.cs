using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Core.ValueTypes;
using ValueOf;

namespace TableTopCrucible.Core.DataAccess.ValueTypes
{
    public class TableName:ValueOf<string, TableName>
    {
        public static TableName FromType<Tid, Tentity>() where Tid:IEntityId where Tentity:IEntity<Tid>
        {
            return new TableName
            {
                Value = typeof(Tentity).FullName
            };
        }
        public RelativeDirectoryPath GetRelativePath()
            => RelativeDirectoryPath.From(this.Value);
    }
}
