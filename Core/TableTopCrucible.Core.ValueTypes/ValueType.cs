using System.Collections.Generic;

using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.ValueTypes
{
    /// <summary>
    ///     used for complex value types like FileHashKey
    /// </summary>
    /// <typeparam name="TThis"></typeparam>
    public abstract class ValueType<TThis> : ComplexDataType
        where TThis : ValueType<TThis>
    {
        public static bool operator ==(ValueType<TThis> valueA, ValueType<TThis> valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;

        public static bool operator !=(ValueType<TThis> valueA, ValueType<TThis> valueB)
            => !(valueA == valueB);

        protected virtual void Validate() { }

        protected virtual bool _Equals(TThis other) => base._Equals(other);
        protected override bool _Equals(ComplexDataType other) => this._Equals(other as TThis);

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
                _value = Sanitize(value);
                Validate();
            }
        }

        public static TThis From(TValue valueA)
            => new() { Value = valueA };

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Value };

        public override string ToString() => Value.ToString();
        protected virtual TValue Sanitize(TValue value) => value;
        protected override bool _Equals(TThis other) => this.Value.Equals(other.Value);

        public static explicit operator ValueType<TValue, TThis>(TValue value)
            => new TThis { Value = value };
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
                Validate();
                _valueA = value;
            }
        }
        private readonly TValueB _valueB;
        public TValueB ValueB
        {
            get => _valueB;
            init
            {
                Validate();
                _valueB = value;
            }
        }

        public static TThis From(TValueA valueA, TValueB valueB)
            => new() { ValueA = valueA, ValueB = valueB };

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { ValueA, ValueB };

        public override string ToString() => $"A: {ValueA} | B: {ValueB}";
        protected override bool _Equals(TThis other) =>
            this.ValueA.Equals(other.ValueA)
            && this.ValueB.Equals(other.ValueB);
    }
}