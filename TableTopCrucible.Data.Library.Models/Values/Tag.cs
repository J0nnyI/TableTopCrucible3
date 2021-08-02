using System;

namespace TableTopCrucible.Data.Library.Models.Values
{
    public struct Tag : IComparable
    {
        private readonly string _tag;
        public Tag(string tag)
        {
            _tag = tag.Trim();
        }
        public override string ToString() => _tag;
        public override bool Equals(object obj)
        {
            return obj switch
            {
                Tag tag => _tag == tag._tag,
                string tag => _tag == tag,
                _ => false,
            };
        }
        public override int GetHashCode() => _tag.GetHashCode();
        public static explicit operator Tag(string tag) => new Tag(tag);
        public static explicit operator string(Tag tag) => tag._tag;
        public static bool operator ==(Tag tag1, Tag tag2)
        {
            return tag1._tag == tag2._tag;
        }
        public static bool operator !=(Tag tag1, Tag tag2)
        {
            return tag1._tag != tag2._tag;
        }

        public int CompareTo(object obj)
            => obj is Tag otherTag ? _tag.CompareTo(otherTag._tag) : 1;


    }
}
