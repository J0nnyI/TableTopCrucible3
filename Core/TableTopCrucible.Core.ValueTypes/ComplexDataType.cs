using System;
using System.Collections.Generic;
using System.Linq;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    /// <summary>
    ///     source: https://levelup.gitconnected.com/using-value-objects-with-entity-framework-core-5cead49dbf9c
    ///     github: https://github.dev/fiseni/ImmutabilitySample/tree/master/src/ImmutabilitySample/Entities
    /// </summary>
    public abstract class ComplexDataType
    {
        protected abstract IEnumerable<object> getAtomicValues();

        public override bool Equals(object obj)
            => obj?.GetType() == GetType()
               && obj is ComplexDataType other
               && _Equals(other);

        protected virtual bool _Equals(ComplexDataType other)
            => getAtomicValues().SequenceEqual(other.getAtomicValues());

        public override int GetHashCode()
            => getAtomicValues()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate(HashCode.Combine);
    }
}