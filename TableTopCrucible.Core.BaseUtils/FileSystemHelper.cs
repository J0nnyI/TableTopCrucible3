using Splat;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace TableTopCrucible.Core.BaseUtils
{
    public static class FileSystemHelper
    {
        private readonly static Lazy<IFileSystem> _fileSystem = new Lazy<IFileSystem>(()
            =>Locator.Current.GetService<IFileSystem>()
            ?? throw new NullReferenceException("Could not get a FileSystemImplementation"));
        public static IFileSystem FileSystem => _fileSystem.Value;
        public static IFile File => FileSystem.File;
        public static IDirectory Directory => FileSystem.Directory;
        public static IFileInfoFactory FileInfo => FileSystem.FileInfo;
        public static IPath Path => FileSystem.Path;
    }
}
