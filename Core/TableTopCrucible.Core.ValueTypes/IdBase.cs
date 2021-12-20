using System;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using ValueOf;

namespace TableTopCrucible.Core.ValueTypes
{
    public abstract class IdBase<TThis> : ValueOf<Guid, TThis>
        where TThis : IdBase<TThis>, new()
    {
        public Guid GetGuid() => Value;

        public static TThis New() => new() { Value = Guid.NewGuid() };

        public static explicit operator IdBase<TThis>(Guid id)
            => From(id);

        protected override void Validate()
        {
            if (Value == default)
                throw new InvalidIdException("Id must not be default");
        }
    }
}