using System.IO;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
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
            => path is null
                ? null
                : From(path.Value);

        public FilePath ToFilePath()
            => FilePath.From(Value);

        public Model3DGroup Load(bool freeze)
            => new ModelImporter().Load(Value,null,true);

        public ModelVisual3D LoadVisual(bool freeze)
            => new() { Content = Load(freeze) };
    }
}