using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement.ValueTypes
{
    public class WorkingDirectoryPath : DirectoryPath
    {
        public static readonly RelativeDirectoryPath RelativeWorkingDirectoryPath = RelativeDirectoryPath.From(@".\~TableTopCrucible WD");
        public static WorkingDirectoryPath ForFile(LibraryFilePath file)
        {
            var path = file.GetDirectoryPath();
            var subPath = path + RelativeWorkingDirectoryPath;
            return new WorkingDirectoryPath
            {
                Value = subPath.Value
            };
        }
        public static new WorkingDirectoryPath From(string value)
            => new WorkingDirectoryPath() { Value = value };
        public static new WorkingDirectoryPath GetTemporaryPath()
            => From(
                    DirectoryPath.GetTemporaryPath().Value +
                    DirectoryName.From(@"TableTopCrucible\TemporaryWorkingDirectory")
                );
    }
}
