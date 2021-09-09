
using System;
using System.IO.Abstractions;
using System.Text.Json;

using TableTopCrucible.Core.ValueTypes.Exceptions;

using ValueOf;

using static TableTopCrucible.Core.Helper.FileSystemHelper;

using Stream = System.IO.Stream;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FilePath<Tthis> : ValueOf<string, Tthis> where Tthis : FilePath<Tthis>, new()
    {
        public FileExtension GetExtension(bool toLower = false) => FileExtension.From(Path.GetExtension(toLower ? Value.ToLower() : Value));
        public bool IsModel()
            => GetExtension().IsModel();
        public bool IsImage()
             => GetExtension().IsImage();
        public bool IsLibrary()
             => GetExtension().IsLibrary();
        public bool IsTable()
             => GetExtension().IsTable();
        public FileType GetFileType()
            => GetExtension().GetFileType();
        public Stream OpenRead() => File.OpenRead(this.Value);
        public void Delete() => File.Delete(Value);
        public void TryDelete()
        {
            if (Exists()) File.Delete(Value);
        }
        public string ReadAllText() => File.ReadAllText(Value);
        public bool Exists() => File.Exists(Value);
        public void WriteAllText(string text)
        {
            try
            {
                File.WriteAllText(Value, text);
            }
            catch (Exception ex)
            {
                throw new FileWriteFailedException(ex);
            }
        }

        public void WriteObject(object data, bool createDirectory = true)
        {
            this.GetDirectoryPath().Create();
            string text;
            try
            {
                text = JsonSerializer.Serialize(data);
            }
            catch (Exception ex)
            {
                throw new SerializationFailedException(ex);
            }
            WriteAllText(text);
        }
        public BareFileName GetFilenameWithoutExtension() => BareFileName.From(Path.GetFileNameWithoutExtension(Value));
        public DirectoryPath GetDirectoryPath() => DirectoryPath.From(Path.GetDirectoryName(Value));
        public IFileInfo GetFileInfo() => FileInfo.FromFileName(Value);
    }
    /// <summary>
    /// the path of a file including its name
    /// </summary>
    public class FilePath : FilePath<FilePath>
    {

    }
}
