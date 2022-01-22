using System;
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

        public override string ToString() => Value.ToString();

        /// <summary>
        ///     base implementation: empty
        /// </summary>
        /// <param name="value">the input value</param>
        /// <returns>the sanitized value</returns>
        protected virtual TValue Sanitize(TValue value) => value;

        public override bool Equals(object other)
            => other is TThis otherValue && Value.Equals(otherValue.Value);

        public override int GetHashCode()
            => Value.GetHashCode();

        public static bool operator ==(ValueType<TValue, TThis> valueA, TThis valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;

        public static bool operator !=(ValueType<TValue, TThis> valueA, TThis valueB)
            => !(valueA == valueB);


        public static explicit operator ValueType<TValue, TThis>(TValue value)
            => From(value);
    }

    public abstract class ValueType<TValueA, TValueB, TThis> : ValueType<TThis>
        where TThis : ValueType<TValueA, TValueB, TThis>, new()
    {
        private readonly TValueA _valueA;

        private readonly TValueB _valueB;

        public TValueA ValueA
        {
            get => _valueA;
            init
            {
                Validate(value, ValueB);
                _valueA = value;
            }
        }

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

        protected virtual void Validate(TValueA valueA, TValueB valueB)
        {
        }

        public override string ToString() => $"A: {ValueA} | B: {ValueB}";

        public override bool Equals(object other)
            => other is TThis otherValue && ValueA.Equals(otherValue.ValueA) &&
               ValueB.Equals(otherValue.ValueB);

        public override int GetHashCode()
            => HashCode.Combine(ValueA, ValueB);

        public static bool operator ==(ValueType<TValueA, TValueB, TThis> valueA, TThis valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;

        public static bool operator !=(ValueType<TValueA, TValueB, TThis> valueA, TThis valueB)
            => !(valueA == valueB);
    }

    public abstract class ValueType<TValueA, TValueB, TValueC, TThis> : ValueType<TThis>
        where TThis : ValueType<TValueA, TValueB, TValueC, TThis>, new()
    {
        private readonly TValueA _valueA;

        private readonly TValueB _valueB;

        private readonly TValueC _valueC;

        public TValueA ValueA
        {
            get => _valueA;
            init
            {
                Validate(value, ValueB, ValueC);
                _valueA = value;
            }
        }

        public TValueB ValueB
        {
            get => _valueB;
            init
            {
                Validate(ValueA, value, ValueC);
                _valueB = value;
            }
        }

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
        }

        public override string ToString() => $"A: {ValueA} | B: {ValueB}";

        public override bool Equals(object other)
            => other is TThis otherValue && ValueA.Equals(otherValue.ValueA) &&
               ValueB.Equals(otherValue.ValueB);

        public override int GetHashCode()
            => HashCode.Combine(ValueA, ValueB);

        public static bool operator ==(ValueType<TValueA, TValueB, TValueC, TThis> valueA, TThis valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;

        public static bool operator !=(ValueType<TValueA, TValueB, TValueC, TThis> valueA, TThis valueB)
            => !(valueA == valueB);
    }
}