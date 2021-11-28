using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public class DirectoryDeletionFailedException : Exception
    {
        internal DirectoryDeletionFailedException(string dir, Exception innerException) : base(
            $"directory '{dir}' could not be deleted", innerException)
        {
        }
    }

    public class DirectoryDeletionFailedException<TDirectoryPath> : DirectoryCreationFailedException
        where TDirectoryPath : DirectoryPath<TDirectoryPath>, new()
    {
        public DirectoryDeletionFailedException(TDirectoryPath dir, Exception innerException) : base(dir.Value,
            innerException)
        {
        }
    }
}