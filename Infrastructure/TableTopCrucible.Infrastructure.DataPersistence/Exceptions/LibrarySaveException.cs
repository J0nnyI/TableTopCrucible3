using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.DataPersistence.Exceptions
{
    public class LibrarySaveException : Exception
    {
        public LibraryFilePath File { get; }

        public LibrarySaveException(LibraryFilePath file, Exception innerException)
            : base($"library {file} could not be saved", innerException)
        {
            File = file;
        }
    }
}
