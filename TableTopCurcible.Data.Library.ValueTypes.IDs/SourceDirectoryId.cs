using System;

using TableTopCrucible.Core.Data;

namespace TableTopCurcible.Data.Library.ValueTypes.IDs
{
    public struct SourceDirectoryId : ITypedId
    {
        private Guid _guid;
        public SourceDirectoryId(Guid guid)
            => _guid = guid;
        public override bool Equals(object obj)
        {
            if (obj is SourceDirectoryId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid() => _guid;

        public static SourceDirectoryId New()
            => (SourceDirectoryId)Guid.NewGuid();

        public override string ToString() => _guid.ToString();
        public static bool operator ==(SourceDirectoryId id1, SourceDirectoryId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(SourceDirectoryId id1, SourceDirectoryId id2)
            => id1._guid != id2._guid;

        public static explicit operator Guid(SourceDirectoryId id)
            => id._guid;
        public static explicit operator SourceDirectoryId(Guid id)
            => new SourceDirectoryId(id);
    }
}
