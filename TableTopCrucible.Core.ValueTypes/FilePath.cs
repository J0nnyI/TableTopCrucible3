
using static TableTopCrucible.Core.BaseUtils.FileSystemHelper;

using ValueOf;
using System.IO.Abstractions;
using Stream = System.IO.Stream;

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
        public bool IsLibrary()
             => GetExtension().IsLibrary();
        public FileType GetFileType()
            => GetExtension().GetFileType();
        public Stream OpenRead() => File.OpenRead(this.Value);
        public void Delete() => File.Delete(Value);
        public string ReadAllText() => File.ReadAllText(Value);
        public bool Exists() => File.Exists(Value);
        public void WriteAllText(string text) => File.WriteAllText(Value, text);
        public BareFileName GetFilenameWithoutExtension() => BareFileName.From(Path.GetFileNameWithoutExtension(Value));
        public DirectoryPath GetDirectoryPath() => DirectoryPath.From(Path.GetDirectoryName(Value));
        public IFileInfo GetFileInfo() => FileInfo.FromFileName(Value);

    }
}
