using ValueOf;

using static TableTopCrucible.Core.BaseUtils.FileSystemHelper;

namespace TableTopCrucible.Core.ValueTypes
{
    public class RelativeDirectoryPath : ValueOf<string, RelativeDirectoryPath>
    {
        public static DirectoryPath operator +(DirectoryPath directory, RelativeDirectoryPath relativeDirectory)
            => DirectoryPath.From(Path.Combine(directory.Value, relativeDirectory.Value));
    }
}
