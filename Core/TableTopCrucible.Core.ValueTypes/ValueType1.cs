using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes;

public abstract class ValueType<TValue, TThis> : ValueType<TThis>
    where TThis : ValueType<TValue, TThis>, new()
{
#pragma warning disable CS8618
    [NotNull]
    private readonly TValue _value;
#pragma warning restore CS8618

    [NotNull]
    public TValue Value
    {
        get => _value;
        init
        {
            Validate(value);
            _value = Sanitize(value);
        }
    }

    public static TThis? From(TValue valueA)
        => valueA is null
            ? null
            : new TThis { Value = valueA };

    /// <summary>
    ///     base implementation: null value check => exception
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="InvalidValueException"></exception>
    protected virtual void Validate(TValue value)
    {
        if (value is null)
            throw new InvalidValueException(nameof(value));
    }

    public override string ToString() => Value.ToString() ?? "NULL";

    /// <summary>
    ///     base implementation: empty
    /// </summary>
    /// <param name="value">the input value</param>
    /// <returns>the sanitized value</returns>
    protected virtual TValue Sanitize(TValue value) => value;

    public override bool Equals(object? other)
        => other is TThis otherValue && Value.Equals(otherValue.Value);

    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(ValueType<TValue, TThis>? valueA, TThis? valueB)
        => valueA is null && valueB is null || valueA?.Equals(valueB) is true;

    public static bool operator !=(ValueType<TValue, TThis>? valueA, TThis? valueB)
        => !(valueA == valueB);


    public static explicit operator ValueType<TValue, TThis>?([NotNull] TValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        return From(value);
    }
}

