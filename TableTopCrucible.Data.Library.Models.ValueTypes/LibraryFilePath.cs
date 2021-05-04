using System;
using System.IO;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

using ValueOf;

namespace TableTopCrucible.Data.Library.Models.ValueTypes
{
    public class LibraryFilePath:FilePath
    {
        protected override void Validate()
        {
            if (!base.IsLibrary())
                throw new InvalidFiletypeException($"{this.Value} is not a valid library file");
            base.Validate();
        }
    }
}
