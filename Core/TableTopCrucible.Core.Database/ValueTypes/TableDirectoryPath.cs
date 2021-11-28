using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Database.ValueTypes
{
    public class TableDirectoryPath : DirectoryPath<TableDirectoryPath>
    {
        public static TableDirectoryPath From(DirectoryPath dirPath) => new() {Value = dirPath.Value};
    }
}