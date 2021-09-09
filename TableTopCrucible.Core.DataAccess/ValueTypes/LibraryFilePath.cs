using System.IO.Compression;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.DataAccess.ValueTypes
{
    public class LibraryFilePath : FilePath<LibraryFilePath>
    {
        protected override void Validate()
        {
            if (!IsLibrary())
                throw new InvalidFiletypeException($"{Value} is not a valid library file");
            base.Validate();
        }

        public LibraryDirectoryPath UnpackLibrary(bool overwriteFiles = false)
        {
            var path = LibraryDirectoryPath.ForFile(this);
            ZipFile.ExtractToDirectory(Value, path.Value, overwriteFiles);
            return path;
        }

        public LibraryDirectoryPath GetWorkingDirectory()
            => LibraryDirectoryPath.ForFile(this);

        public new LibraryDirectoryPath GetDirectoryPath() => LibraryDirectoryPath.From(base.GetDirectoryPath());
    }
}
