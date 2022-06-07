using TableTopCrucible.Core.ValueTypes.Exceptions;

using static TableTopCrucible.Core.Helper.FileSystemHelper;

namespace TableTopCrucible.Core.ValueTypes;

public class RelativeFilePath : ValueType<string, RelativeFilePath>
{
    public static RelativeFilePath operator +(RelativeDirectoryPath directory, RelativeFilePath relativeFilePath) 
        => From(Path.Combine(directory.Value, relativeFilePath.Value));
    public static FilePath operator +(DirectoryPath directory, RelativeFilePath relativeFilePath) 
        => FilePath.From(Path.Combine(directory.Value, relativeFilePath.Value));

    public FileExtension GetExtension()
        => (FileExtension)Path.GetExtension(Value);

    public RelativeDirectoryPath GetDirectoryName()
        => (RelativeDirectoryPath)Path.GetDirectoryName(Value);

    protected override void Validate(string value)
    {
        
        base.Validate(value);
        if (Path.IsPathRooted(value))
            throw new InvalidValueException("the past must not be rooted");
        if (value.EndsWith(Path.AltDirectorySeparatorChar) || value.EndsWith(Path.DirectorySeparatorChar))
            throw new InvalidValueException("a relative Filepath must not end with a " + Path.DirectorySeparatorChar);
    }
}