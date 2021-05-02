
using System.IO;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    /// the path of a file including its name
    /// </summary>
    public class FilePath : ValueOf<string, FilePath>
    {
        public FileExtension GetExtension(bool toLower = false) => FileExtension.From(Path.GetExtension(toLower ? Value.ToLower() : Value));
        public bool IsModel()
            => GetExtension().IsModel();
        public bool IsImage()
             => GetExtension().IsImage();
        public FileType GetFileType()
            => GetExtension().GetFileType();
    }
}
