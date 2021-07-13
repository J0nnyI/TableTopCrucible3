using System;
using System.Windows;

using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement.ValueTypes
{
    public class TableDirectoryPath : DirectoryPath<TableDirectoryPath>
    {
        public static TableDirectoryPath From(DirectoryPath dirPath)
            => new TableDirectoryPath() { Value = dirPath.Value };
    }
}
