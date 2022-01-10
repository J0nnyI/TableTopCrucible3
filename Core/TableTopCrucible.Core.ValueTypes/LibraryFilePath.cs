using System.IO;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class LibraryFilePath : FilePath<LibraryFilePath>
    {
        public static LibraryFilePath WorkingFile
            => (LibraryFilePath)(DirectoryPath.AppData + (FileName)"workingDatabase.ttcl");

        public bool IsWorkingFile
            => this == WorkingFile;

        protected override void Validate(string value)
        {
            base.Validate(value);
            if (!FileExtension.From(Path.GetExtension(value)).IsLibrary())
                throw new InvalidFileTypeException($"{value} is not a valid library file");
        }

        public static explicit operator LibraryFilePath(FilePath value)
            => From(value.Value);
    }
}