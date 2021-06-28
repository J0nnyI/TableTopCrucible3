using System;
using System.Windows;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement.ValueTypes
{
    public class WorkingDirectoryPath : DirectoryPath<WorkingDirectoryPath>
    {
        public static readonly RelativeDirectoryPath RelativeWorkingDirectoryPath = RelativeDirectoryPath.From(@".\~TableTopCrucible WD");
        public static WorkingDirectoryPath ForFile(LibraryFilePath file)
        {
            var path = file.GetDirectoryPath();
            var subPath = path + RelativeWorkingDirectoryPath;
            return new WorkingDirectoryPath
            {
                Value = subPath.Value
            };
        }
        public static WorkingDirectoryPath From(DirectoryPath dirPath)
            => new WorkingDirectoryPath() { Value = dirPath.Value };
        public static  WorkingDirectoryPath GetTemporaryPath()
            => From(
                    DirectoryPath.AppData +
                    DirectoryName.From(@"TableTopCrucible\TemporaryWorkingDirectory")
                );
    }
}
