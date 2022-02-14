using System;
using System.Collections.Generic;
using System.Linq;
using static TableTopCrucible.Core.Helper.FileSystemHelper;

namespace TableTopCrucible.Core.ValueTypes;

public class RelativeDirectoryPath : ValueType<string, RelativeDirectoryPath>
{
    public static DirectoryPath operator +(DirectoryPath directory, RelativeDirectoryPath relativeDirectory) =>
        DirectoryPath.From(Path.Combine(directory.Value, relativeDirectory.Value));

    public IEnumerable<DirectoryName> GetDirectoryNames()
        => Value.Split(System.IO.Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(DirectoryName.From);
}