
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

        protected override void Validate()
        {
            if (Value == null)
                throw new InvalidValueException(nameof(Value));
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

        public void SetCreationTime(DateTime time)
            => File.SetCreationTime(Value, time);

        public IFileInfo GetInfo() => FileInfo.FromFileName(Value);
        // uses GetInfo, if you need other properties use that method instead
        public FileSize GetSize() => FileSize.From(GetInfo().Length);

        public override int GetHashCode()
            => Value.ToLower().GetHashCode();

        public override bool Equals(object obj)
            => obj is FilePath<Tthis> other &&
               Value.ToLower().Equals(other.Value.ToLower());

        public static bool operator ==(FilePath<Tthis> a, Tthis b)
            => (a is null && b is null) || // both null
               (a is not null && b is not null && a.Equals(b)); // both filled

        public static bool operator !=(FilePath<Tthis> a, Tthis b)
            => !(a == b);
    }
    /// <summary>
    /// the path of a file including its name
    /// </summary>
    public class FilePath : FilePath<FilePath>
    {

    }
}
