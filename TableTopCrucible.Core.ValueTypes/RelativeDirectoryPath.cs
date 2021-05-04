using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class RelativeDirectoryPath : ValueOf<string, RelativeDirectoryPath>
    {
        public static DirectoryPath operator +(DirectoryPath directory, RelativeDirectoryPath relativeDirectory)
            => DirectoryPath.From(Path.Combine(directory, relativeDirectory));
    }
}
