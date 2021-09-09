using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.DataAccess.ValueTypes
{
    public class TableDirectoryPath : DirectoryPath<TableDirectoryPath>
    {
        public static TableDirectoryPath From(DirectoryPath dirPath)
            => new TableDirectoryPath() { Value = dirPath.Value };
    }
}
