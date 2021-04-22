﻿using System.IO;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    /// the path of a directory
    /// </summary>
    public class DirectoryPath : ValueOf<string, DirectoryPath>
    {
        public static FilePath operator +(DirectoryPath directory, FileName fileName)
            => FilePath.From(Path.Combine(directory, fileName));
    }
}