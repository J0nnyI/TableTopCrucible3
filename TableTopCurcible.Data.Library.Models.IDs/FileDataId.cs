using System;

using TableTopCrucible.Core.Data;

namespace TableTopCurcible.Data.Library.Models.IDs
{
    public struct FileDataId : ITypedId
    {
        private Guid _guid;
        public FileDataId(Guid guid)
            => _guid = guid;

        public override bool Equals(object obj)
        {
            if (obj is FileDataId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid()
            => _guid;

        public static bool operator ==(FileDataId id1, FileDataId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(FileDataId id1, FileDataId id2)
            => id1._guid != id2._guid;
        public static explicit operator Guid(FileDataId id)
            => id._guid;
        public static explicit operator FileDataId(Guid id)
            => new FileDataId(id);
        public override string ToString()
            => _guid.ToString();
        public static FileDataId New()
            => (FileDataId)Guid.NewGuid();

    }
}
