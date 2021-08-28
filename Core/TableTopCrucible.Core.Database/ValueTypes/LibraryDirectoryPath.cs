using System.IO;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Database.ValueTypes
{
    public class LibraryDirectoryPath : DirectoryPath<LibraryDirectoryPath>
    {
        public static readonly RelativeDirectoryPath RelativeWorkingDirectoryPath = RelativeDirectoryPath.From(@"~TableTopCrucible");

        public static LibraryDirectoryPath From(DirectoryPath dirPath)
            => new LibraryDirectoryPath() { Value = dirPath.Value };

        public static LibraryDirectoryPath ForFile(LibraryFilePath file)
        => new LibraryDirectoryPath
        {
            Value = Path.Combine(file.GetDirectoryPath().Value, RelativeWorkingDirectoryPath.Value)
        };
        public static LibraryDirectoryPath GetTemporaryPath()
            => From(
                    DirectoryPath.AppData +
                    DirectoryName.From(@"TableTopCrucible\TemporaryWorkingDirectory")
                );
    }
}
