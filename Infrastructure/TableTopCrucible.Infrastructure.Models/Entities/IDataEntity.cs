using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    /// <summary>
    /// source: https://levelup.gitconnected.com/using-value-objects-with-entity-framework-core-5cead49dbf9c
    /// </summary>
    public abstract class ComplexValueType
    {
        protected static bool EqualOperator(ComplexValueType left, ComplexValueType right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ComplexValueType left, ComplexValueType right)
        {
            return !(EqualOperator(left, right));
        }

        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            ComplexValueType other = (ComplexValueType)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^
                    ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }

                if (thisValues.Current != null &&
                    !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
        // Other utility methods
    }

    public interface IDataEntity<TId>
    where TId : IDataId
    {
        TId Id { get; init; }
    }

    public abstract class DataEntity<TId> : IDataEntity<TId>
        where TId : IDataId
    {
        public TId Id { get; init; }
    }
}