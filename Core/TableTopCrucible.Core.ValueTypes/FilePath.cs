using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FilePath<TThis> : ValueType<string, TThis> where TThis : FilePath<TThis>, new()
    {
        public FileExtension GetExtension(bool toLower = false) =>
            FileExtension.From(FileSystemHelper.Path.GetExtension(toLower
                ? Value.ToLower()
                : Value));

        public bool IsModel() => GetExtension().IsModel();

        public bool IsImage() => GetExtension().IsImage();

        public bool IsLibrary() => GetExtension().IsLibrary();

        public bool IsTable() => GetExtension().IsTable();

        public FileType GetFileType() => GetExtension().GetFileType();

        public Stream OpenRead() => FileSystemHelper.File.OpenRead(Value);
        public Stream OpenWrite() => FileSystemHelper.File.OpenWrite(Value);

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

        public void Copy(FilePath<TThis> newPath, bool overwrite = true)
        {
            if(this.Exists())
                FileSystemHelper.File.Copy(Value, newPath.Value, overwrite);
        }

        public bool IsLocatedIn(DirectoryPath directory)
            => directory.Value.ToLower() == Value.ToLower();

        public bool IsLocatedInAny(params DirectoryPath[] directories)
            => directories.Any(IsLocatedIn);

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

        public Uri ToUri()
            => new(Value);
        public IFileInfo GetInfo() => FileSystemHelper.FileInfo.FromFileName(Value);

        // uses GetInfo, if you need other properties use that method instead
        public FileSize GetSize() => FileSize.From(GetInfo().Length);
        public DateTime GetLastWriteTime() => GetInfo().LastWriteTime;

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