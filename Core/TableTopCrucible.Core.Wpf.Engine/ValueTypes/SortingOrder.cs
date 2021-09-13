using System;

using ValueOf;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class SortingOrder : ValueOf<decimal, SortingOrder>, IComparable, IComparable<SortingOrder>
    {
        public int CompareTo(object obj)
            => obj == null ? 1 : Value.CompareTo(obj);

        public int CompareTo(SortingOrder other)
            => other == null ? 1 : Value.CompareTo(other.Value);
    }
}
