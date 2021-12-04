using System.IO.Compression;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.Database.ValueTypes
{
    public class LibraryFilePath : FilePath<LibraryFilePath>
    {
        protected override void Validate()
        {
            if (!IsLibrary())
                throw new InvalidFileTypeException($"{Value} is not a valid library file");
            base.Validate();
        }
    }
}