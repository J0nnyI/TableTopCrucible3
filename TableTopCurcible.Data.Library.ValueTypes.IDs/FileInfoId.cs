using System;

using TableTopCrucible.Core.Data;

namespace TableTopCurcible.Data.Library.ValueTypes.IDs
{
    public struct FileInfoId : ITypedId
    {
        private Guid _guid;
        public FileInfoId(Guid guid)
            => _guid = guid;

        public override bool Equals(object obj)
        {
            if (obj is FileInfoId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid()
            => _guid;

        public static bool operator ==(FileInfoId id1, FileInfoId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(FileInfoId id1, FileInfoId id2)
            => id1._guid != id2._guid;
        public static explicit operator Guid(FileInfoId id)
            => id._guid;
        public static explicit operator FileInfoId(Guid id)
            => new FileInfoId(id);
        public override string ToString()
            => _guid.ToString();
        public static FileInfoId New()
            => (FileInfoId)Guid.NewGuid();

    }
}
