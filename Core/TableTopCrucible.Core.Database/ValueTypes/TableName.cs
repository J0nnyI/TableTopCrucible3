using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using ValueOf;

namespace TableTopCrucible.Core.Database.ValueTypes
{
    public class TableName : ValueOf<string, TableName>
    {
        public static TableName FromType<Tid, Tentity>() where Tid : IEntityId where Tentity : IEntity<Tid> =>
            new()
            {
                Value = typeof(Tentity).FullName
            };

        public RelativeDirectoryPath GetRelativePath() => RelativeDirectoryPath.From(Value);
    }
}