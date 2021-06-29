using System.IO.Compression;

using TableTopCrucible.Core.DataAccess.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.FileManagement.ValueTypes
{
    public class TableFilePath : FilePath<TableFilePath>
    {
        protected override void Validate()
        {
            if (!IsLibrary())
                throw new InvalidFiletypeException($"{Value} is not a valid library file");
            base.Validate();
        }

        public static TableFilePath From(FilePath path)
            => From(path.Value);

        public static TableFilePath From(WorkingDirectoryPath workingDirectory, TableSaveId tableId, TableName tableName)
            => From(workingDirectory + tableName.GetRelativePath() + (tableId.GetBareFilename() + FileExtension.Table));
    }
}
