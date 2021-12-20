using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class LibraryFilePath : FilePath<LibraryFilePath>
    {
        protected override void Validate()
        {
            if (!IsLibrary())
                throw new InvalidFileTypeException($"{Value} is not a valid library file");
            base.Validate();
        }

        public static LibraryFilePath WorkingFile
            => (LibraryFilePath)(DirectoryPath.AppData + (FileName) "workingDatabase.ttcl");

        public static explicit operator LibraryFilePath(FilePath value)
            => From(value.Value);

        public bool IsWorkingFile
            => this == WorkingFile;
    }
}