using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes;

public abstract class ValueType<TValueA, TValueB, TThis> : ValueType<TThis>
    where TThis : ValueType<TValueA, TValueB, TThis>, new()
{
#pragma warning disable CS8618
    [NotNull]
    private readonly TValueA _valueA;

    [NotNull]
    private readonly TValueB _valueB;
#pragma warning restore CS8618

    [NotNull]
    public TValueA ValueA
    {
        get => _valueA;
        init
        {
            Validate(value, ValueB);
            _valueA = value;
        }
    }

    [NotNull]
    public TValueB ValueB
    {
        get => _valueB;
        init
        {
            Validate(ValueA, value);
            _valueB = value;
        }
    }

    public static TThis From(TValueA valueA, TValueB valueB)
        => new() { ValueA = valueA, ValueB = valueB };

    protected virtual void Validate(TValueA? valueA, TValueB? valueB)
    {
        if (valueA is null)
            throw new NullReferenceException(nameof(valueA));
        if (valueB is null)
            throw new NullReferenceException(nameof(valueB));
    }

    public override string ToString() => $"A: {ValueA} | B: {ValueB}";

    public override bool Equals(object? other)
        => other is TThis otherValue &&
           ValueA.Equals(otherValue.ValueA) && ValueB.Equals(otherValue.ValueB);

    public override int GetHashCode()
        => HashCode.Combine(ValueA, ValueB);

    public static bool operator ==(ValueType<TValueA, TValueB, TThis>? valueA, TThis? valueB)
        => valueA is null && valueB is null || valueA?.Equals(valueB) is true;

    public static bool operator !=(ValueType<TValueA, TValueB, TThis>? valueA, TThis? valueB)
        => !(valueA == valueB);
}
