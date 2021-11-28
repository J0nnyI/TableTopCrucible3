using System;

namespace TableTopCrucible.Core.ValueTypes.Exceptions
{
    public abstract class DirectoryMoveFailedException : Exception
    {
        protected DirectoryMoveFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class DirectoryMoveFailedException<TDir> : DirectoryMoveFailedException
    {
        public DirectoryMoveFailedException(TDir oldDirectory, TDir newDirectory, Exception innerException) : base(
            $"directory '{oldDirectory}' could not be renamed to '{newDirectory}'", innerException)
        {
            OldDirectory = oldDirectory;
            NewDirectory = newDirectory;
        }

        public TDir OldDirectory { get; }
        public TDir NewDirectory { get; }
    }
}