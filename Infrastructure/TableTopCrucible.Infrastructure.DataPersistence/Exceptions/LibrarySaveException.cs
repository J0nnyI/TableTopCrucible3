using System;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.DataPersistence.Exceptions
{
    public class LibrarySaveException : Exception
    {
        public LibrarySaveException(LibraryFilePath file, Exception innerException)
            : base($"library {file} could not be saved", innerException)
        {
            File = file;
        }

        public LibraryFilePath File { get; }
    }
}