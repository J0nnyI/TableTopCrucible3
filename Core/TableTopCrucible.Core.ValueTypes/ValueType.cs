using System;
using System.Collections.Generic;

using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    ///     used for complex value types like FileHashKey
    /// </summary>
    /// <typeparam name="TThis"></typeparam>
    public abstract class ValueType<TThis>
        where TThis : ValueType<TThis>
    {
    }

    public abstract class ValueType<TValue, TThis> : ValueType<TThis>
        where TThis : ValueType<TValue, TThis>, new()
    {
        private readonly TValue _value;
        public TValue Value
        {
            get => _value;
            init
            {
                Validate(value);
                _value = Sanitize(value);
            }
        }

        public static TThis From(TValue valueA)
            => new() { Value = valueA };


        protected virtual void Validate(TValue value)
        {
            if (value is null)
                throw new InvalidValueException(nameof(value));
        }
        public override string ToString() => Value.ToString();
        protected virtual TValue Sanitize(TValue value) => value;

        public override bool Equals(object other)
            => other is TThis otherValue && this.Value.Equals(otherValue.Value);
        public override int GetHashCode()
            => Value.GetHashCode();
        public static bool operator ==(ValueType<TValue, TThis> valueA, TThis valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;
        public static bool operator !=(ValueType<TValue, TThis> valueA, TThis valueB)
            => !(valueA == valueB);


        public static explicit operator ValueType<TValue, TThis>(TValue value)
            => value is null ? null : new TThis { Value = value };
    }

    public abstract class ValueType<TValueA, TValueB, TThis> : ValueType<TThis>
        where TThis : ValueType<TValueA, TValueB, TThis>, new()
    {
        private readonly TValueA _valueA;
        public TValueA ValueA
        {
            get => _valueA;
            init
            {
                Validate(value, ValueB);
                _valueA = value;
            }
        }
        private readonly TValueB _valueB;
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

        protected virtual void Validate(TValueA valueA, TValueB valueB) { }
        public override string ToString() => $"A: {ValueA} | B: {ValueB}";

        public override bool Equals(object other)
            => other is TThis otherValue && this.ValueA.Equals(otherValue.ValueA) && this.ValueB.Equals(otherValue.ValueB);
        public override int GetHashCode()
            => HashCode.Combine(ValueA, ValueB);
        public static bool operator ==(ValueType<TValueA, TValueB, TThis> valueA, TThis valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;
        public static bool operator !=(ValueType<TValueA, TValueB, TThis> valueA, TThis valueB)
            => !(valueA == valueB);
    }
}