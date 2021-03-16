using System;

using TableTopCrucible.Core.Data;

namespace TableTopCurcible.Data.Library.Models.IDs
{
    public struct ItemVersionId : ITypedId
    {
        private Guid _guid;
        public ItemVersionId(Guid guid)
            => _guid = guid;
        public override bool Equals(object obj)
        {
            if (obj is ItemVersionId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid() => _guid;

        public static ItemVersionId New()
            => (ItemVersionId)Guid.NewGuid();

        public override string ToString() => _guid.ToString();
        public static bool operator ==(ItemVersionId id1, ItemVersionId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(ItemVersionId id1, ItemVersionId id2)
            => id1._guid != id2._guid;

        public static explicit operator Guid(ItemVersionId id)
            => id._guid;
        public static explicit operator ItemVersionId(Guid id)
            => new ItemVersionId(id);
    }
}
