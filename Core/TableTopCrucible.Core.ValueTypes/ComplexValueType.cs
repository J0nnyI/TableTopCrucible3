using System.Collections.Generic;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    /// <summary>
    ///     used for complex value types like FileHashKey
    /// </summary>
    /// <typeparam name="TThis"></typeparam>
    public abstract class ComplexValueType<TThis> : ComplexDataType
        where TThis : ComplexValueType<TThis>
    {
        public static bool operator ==(ComplexValueType<TThis> valueA, ComplexValueType<TThis> valueB)
            => valueA is null && valueB is null
               || valueA?.Equals(valueB) == true;

        public static bool operator !=(ComplexValueType<TThis> valueA, ComplexValueType<TThis> valueB)
            => !(valueA == valueB);
    }

    public abstract class ComplexValueType<TValueA, TValueB, TThis> : ComplexValueType<TThis>
        where TThis : ComplexValueType<TValueA, TValueB, TThis>, new()
    {
        public TValueA ValueA { get; init; }
        public TValueB ValueB { get; init; }

        public static TThis From(TValueA valueA, TValueB valueB)
            => new() { ValueA = valueA, ValueB = valueB };

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { ValueA, ValueB };

        public override string ToString() => $"A: {ValueA} | B: {ValueB}";
    }
}