using TableTopCrucible.Core.ValueTypes.Exceptions;
using static TableTopCrucible.Core.Helper.FileSystemHelper;

namespace TableTopCrucible.Core.ValueTypes;

/// <summary>
///     the name of a file without a path but with extension
/// </summary>
public class FileName : ValueType<string, FileName>
{
    public FileExtension GetExtension(bool toLower = false) =>
        FileExtension.From(Path.GetExtension(toLower
            ? Value.ToLower()
            : Value));

    public bool IsModel() => GetExtension().IsModel();

    public bool IsImage() => GetExtension().IsImage();

    public FileType GetFileType() => GetExtension().GetFileType();

    public static explicit operator FileName(string value)
        => From(value);

    protected override void Validate(string value)
    {
        base.Validate(value);
        if (!Path.HasExtension(value))
            throw new InvalidValueException($"The given name '{value}' does not contain an extension");
    }
}