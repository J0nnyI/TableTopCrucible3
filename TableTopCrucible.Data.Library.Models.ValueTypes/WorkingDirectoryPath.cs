using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Data.Library.Models.ValueTypes
{
    public class WorkingDirectoryPath : DirectoryPath
    {
        public static readonly RelativeDirectoryPath RelativeWorkingDirectoryPath = RelativeDirectoryPath.From(@".\~TableTopCrucible WD");
        public static WorkingDirectoryPath ForFile(LibraryFilePath file)
        {
            var path = DirectoryPath.From(Path.GetDirectoryName(file));
            var subPath = path + RelativeWorkingDirectoryPath;
            return new WorkingDirectoryPath
            {
                Value = subPath.Value
            };
        }
    }
}
