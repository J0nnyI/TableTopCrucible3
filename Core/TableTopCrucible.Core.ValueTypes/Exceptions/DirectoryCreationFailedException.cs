using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class DirectoryCreationFailedException: Exception
    {
        internal DirectoryCreationFailedException(string dir, Exception innerException) : base($"directory '{dir}' could not be created", innerException)
        {

        }
    }
    public class DirectoryCreationFailedException<TDirectoryPath> : DirectoryCreationFailedException where TDirectoryPath : DirectoryPath<TDirectoryPath>, new()
    {
        public DirectoryCreationFailedException(TDirectoryPath dir, Exception innerException) : base(dir.Value, innerException)
        {

        }
    }
}
