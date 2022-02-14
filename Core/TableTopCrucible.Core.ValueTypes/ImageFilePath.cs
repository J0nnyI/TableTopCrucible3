using System.IO;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes;

public class ImageFilePath : FilePath<ImageFilePath>
{
    protected override void Validate(string value)
    {
        base.Validate(value);
        if (!FileExtension.From(Path.GetExtension(value)).IsImage())
            throw new InvalidFileTypeException($"Path {value} is not a valid image file");
    }

    public static ImageFilePath From(FilePath path)
        => path is null
            ? null
            : From(path.Value);

    public FilePath ToFilePath()
        => FilePath.From(Value);
}