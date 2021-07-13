using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.DataAccess.ValueTypes
{
    public class LibraryDirectoryPath : DirectoryPath<LibraryDirectoryPath>
    {
        public static readonly RelativeDirectoryPath RelativeWorkingDirectoryPath = RelativeDirectoryPath.From(@".\~TableTopCrucible WD");
        public static LibraryDirectoryPath ForFile(LibraryFilePath file)
        {
            var path = file.GetDirectoryPath();
            var subPath = path + RelativeWorkingDirectoryPath;
            return new LibraryDirectoryPath
            {
                Value = subPath.Value
            };
        }
        public static LibraryDirectoryPath From(DirectoryPath dirPath)
            => new LibraryDirectoryPath() { Value = dirPath.Value };
        public static  LibraryDirectoryPath GetTemporaryPath()
            => From(
                    DirectoryPath.AppData +
                    DirectoryName.From(@"TableTopCrucible\TemporaryWorkingDirectory")
                );
    }
}
