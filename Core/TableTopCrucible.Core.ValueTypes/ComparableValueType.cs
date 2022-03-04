using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes;

/// <summary>
/// additional functionality for atomic types like int, datetime or string
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TThis"></typeparam>
public abstract class ComparableValueType<TValue, TThis>
    : ValueType<TValue, TThis>, IComparable, IComparable<TThis>
    where TThis : ValueType<TValue, TThis>, new()
    where TValue : IComparable
{
    public int CompareTo(TThis other)
        => other is null
        ? 1
        : Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) 
        => CompareTo(obj as TThis);
}