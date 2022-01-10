using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class ModelFilePath : FilePath<ModelFilePath>
    {
        protected override void Validate(string value)
        {
            base.Validate(value);
            if (!FileExtension.From(Path.GetExtension(value)).IsModel())
                throw new InvalidFileTypeException($"Path {value} is not a valid model file");
        }

        public static ModelFilePath From(FilePath path)
            => path is null ? null : From(path.Value);
    }
}
