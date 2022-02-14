using System.IO;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes;

public class LibraryFilePath : FilePath<LibraryFilePath>
{
    public static LibraryFilePath DefaultFile
        => (LibraryFilePath)(DirectoryPath.AppData + ((BareFileName)"workingDatabase" + FileExtension.JSonLibrary));

    public bool IsWorkingFile
        => this == DefaultFile;

    protected override void Validate(string value)
    {
        base.Validate(value);
        if (!FileExtension.From(Path.GetExtension(value)).IsLibrary())
            throw new InvalidFileTypeException($"{value} is not a valid library file");
    }

    public static explicit operator LibraryFilePath(FilePath value)
        => From(value.Value);
}