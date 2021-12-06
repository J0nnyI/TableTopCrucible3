using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FilePath<TThis> : ValueOf<string, TThis> where TThis : FilePath<TThis>, new()
    {
        public FileExtension GetExtension(bool toLower = false) =>
            FileExtension.From(FileSystemHelper.Path.GetExtension(toLower ? Value.ToLower() : Value));

        public bool IsModel() => GetExtension().IsModel();

        public bool IsImage() => GetExtension().IsImage();

        public bool IsLibrary() => GetExtension().IsLibrary();

        public bool IsTable() => GetExtension().IsTable();

        public FileType GetFileType() => GetExtension().GetFileType();

        public Stream OpenRead() => FileSystemHelper.File.OpenRead(Value);

        public void Delete()
        {
            FileSystemHelper.File.Delete(Value);
        }

        public void TryDelete()
        {
            if (Exists()) FileSystemHelper.File.Delete(Value);
        }

        public string ReadAllText() => FileSystemHelper.File.ReadAllText(Value);

        public bool Exists() => FileSystemHelper.File.Exists(Value);

        public void WriteAllText(string text)
        {
            try
            {
                FileSystemHelper.File.WriteAllText(Value, text);
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
            GetDirectoryPath().Create();
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

        public BareFileName GetFilenameWithoutExtension() =>
            BareFileName.From(FileSystemHelper.Path.GetFileNameWithoutExtension(Value));

        public DirectoryPath GetDirectoryPath() => DirectoryPath.From(FileSystemHelper.Path.GetDirectoryName(Value));

        public void SetCreationTime(DateTime time)
        {
            FileSystemHelper.File.SetCreationTime(Value, time);
        }

        public IFileInfo GetInfo() => FileSystemHelper.FileInfo.FromFileName(Value);

        // uses GetInfo, if you need other properties use that method instead
        public FileSize GetSize() => FileSize.From(GetInfo().Length);

        public override int GetHashCode() => Value.ToLower().GetHashCode();

        public override bool Equals(object obj) =>
            obj is FilePath<TThis> other &&
            Value.ToLower().Equals(other.Value.ToLower());

        public static bool operator ==(FilePath<TThis> a, TThis b) =>
            a is null && b is null || // both null
            a is not null && b is not null && a.Equals(b); // both filled

        public static bool operator !=(FilePath<TThis> a, TThis b) => !(a == b);

        public static explicit operator FilePath<TThis>(string path)
            => From(path);
            
    }

    /// <summary>
    ///     the path of a file including its name
    /// </summary>
    public class FilePath : FilePath<FilePath>
    {
    }
}