using System;

using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public class FileHashKey : ValueOf<(FileHash, FileSize), FileHashKey>
    {
        public FileHash Hash => Value.Item1;
        public FileSize FileSize => Value.Item2;
        public static FileHashKey From(FileHash hash, FileSize fileSize)
            => From((hash, fileSize));
        public static FileHashKey Create(FilePath file)
            => From((FileHash.Create(file), file.GetSize()));
        public override bool Equals(object obj)
        {
            return obj is FileHashKey key &&
                Value.Item1.Equals(key.Value.Item1) &&
                Value.Item2 == key.Value.Item2;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Value.Item1, Value.Item2);
        }

        public override string ToString()
            => $"S: {FileSize} | H: {Hash}";

    }

}
