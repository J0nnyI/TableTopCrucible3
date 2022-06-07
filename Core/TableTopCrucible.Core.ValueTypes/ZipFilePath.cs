using System.IO;

using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes;

public class ZipFilePath : FilePath<ZipFilePath>
{
    protected override void Validate(string value)
    {
        base.Validate(value);
        if (!((FileExtension)Path.GetExtension(value))?.IsArchive() is true)
            throw new InvalidFileTypeException();
    }
}
