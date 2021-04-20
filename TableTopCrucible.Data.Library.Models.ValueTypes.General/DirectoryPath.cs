﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ValueOf;

namespace TableTopCrucible.Data.Library.Models.ValueTypes.General
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
