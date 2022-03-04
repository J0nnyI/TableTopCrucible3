using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes;
public abstract class ValueType<TValueA, TValueB, TValueC, TThis> : ValueType<TThis>
    where TThis : ValueType<TValueA, TValueB, TValueC, TThis>, new()
{
#pragma warning disable CS8618
    [NotNull]
    private readonly TValueA _valueA;

    [NotNull]
    private readonly TValueB _valueB;

    [NotNull]
    private readonly TValueC _valueC;
#pragma warning restore CS8618

    [NotNull]
    public TValueA ValueA
    {
        get => _valueA;
        init
        {
            Validate(value, ValueB, ValueC);
            _valueA = value;
        }
    }

    [NotNull]
    public TValueB ValueB
    {
        get => _valueB;
        init
        {
            Validate(ValueA, value, ValueC);
            _valueB = value;
        }
    }

    [NotNull]
    public TValueC ValueC
    {
        get => _valueC;
        init
        {
            Validate(ValueA, ValueB, value);
            _valueC = value;
        }
    }

    public static TThis From(TValueA valueA, TValueB valueB, TValueC valueC)
        => new() { ValueA = valueA, ValueB = valueB, ValueC = valueC };

    protected virtual void Validate(TValueA valueA, TValueB valueB, TValueC valueC)
    {
        if (valueA is null)
            throw new NullReferenceException(nameof(valueA));
        if (valueB is null)
            throw new NullReferenceException(nameof(valueB));
        if (valueC is null)
            throw new NullReferenceException(nameof(valueC));
    }

    public override string ToString() => $"A: {ValueA} | B: {ValueB}";

    public override bool Equals(object? other)
        => other is TThis otherValue
           && ValueA.Equals(otherValue.ValueA)
           && ValueB.Equals(otherValue.ValueB)
           && ValueC.Equals(otherValue.ValueC);

    public override int GetHashCode()
        => HashCode.Combine(ValueA, ValueB);

    public static bool operator ==(ValueType<TValueA, TValueB, TValueC, TThis>? valueA, TThis? valueB)
        => valueA is null && valueB is null || valueA?.Equals(valueB) is true;

    public static bool operator !=(ValueType<TValueA, TValueB, TValueC, TThis>? valueA, TThis? valueB)
        => !(valueA == valueB);
}