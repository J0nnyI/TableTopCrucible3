﻿using System;

using TableTopCrucible.Core.Data;

namespace TableTopCrucible.Data.Library.Models.IDs
{
    public struct ItemId : ITypedId
    {
        private Guid _guid;
        public ItemId(Guid guid)
            => _guid = guid;
        public override bool Equals(object obj)
        {
            if (obj is ItemId id)
                return _guid == id._guid;
            return false;
        }

        public override int GetHashCode() => _guid.GetHashCode();
        public Guid ToGuid() => _guid;
        public static ItemId New() => (ItemId)Guid.NewGuid();
        public override string ToString() => _guid.ToString();
        public static bool operator ==(ItemId id1, ItemId id2)
            => id1._guid == id2._guid;
        public static bool operator !=(ItemId id1, ItemId id2)
            => id1._guid != id2._guid;

        public static explicit operator Guid(ItemId id)
            => id._guid;
        public static explicit operator ItemId(Guid id)
            => new ItemId(id);
    }
}
