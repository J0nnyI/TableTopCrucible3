using System;

using TableTopCrucible.Core.Data;

namespace TableTopCurcible.Data.Library.ValueTypes.IDs
{
    public struct FileItemLinkId : ITypedId
    {
        private Guid _guid;
        public FileItemLinkId(Guid guid)
            => _guid = guid;
        public override bool Equals(object obj)
        {
            if (obj is FileItemLinkId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid() => _guid;

        public static FileItemLinkId New()
            => (FileItemLinkId)Guid.NewGuid();

        public override string ToString() => _guid.ToString();
        public static bool operator ==(FileItemLinkId id1, FileItemLinkId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(FileItemLinkId id1, FileItemLinkId id2)
            => id1._guid != id2._guid;

        public static explicit operator Guid(FileItemLinkId id)
            => id._guid;
        public static explicit operator FileItemLinkId(Guid id)
            => new FileItemLinkId(id);
    }
}
