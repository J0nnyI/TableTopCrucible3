using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
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