using System;
using System.IO.Abstractions;
using Splat;

namespace TableTopCrucible.Core.Helper;

public static class FileSystemHelper
{
    private static readonly Lazy<IFileSystem> _fileSystem = new(()
        => Locator.Current.GetService<IFileSystem>() ?? new FileSystem());
    //?? throw new NullReferenceException("Could not get a FileSystemImplementation"));

    public static IFileSystem FileSystem => _fileSystem.Value;
    public static IFile File => FileSystem.File;
    public static IDirectory Directory => FileSystem.Directory;
    public static IFileInfoFactory FileInfo => FileSystem.FileInfo;
    public static IPath Path => FileSystem.Path;
}