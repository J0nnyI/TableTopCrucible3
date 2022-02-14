using System;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.DataPersistence.Exceptions;

public class LibraryLoadException : Exception
{
    public LibraryLoadException(LibraryFilePath file, Exception innerException)
        : base($"library {file} could not be loaded", innerException)
    {
        File = file;
    }

    public LibraryFilePath File { get; }
}