using System;
using TableTopCrucible.Core.Database.Exceptions;
using ValueOf;

namespace TableTopCrucible.Core.Database.Models
{
    public interface IEntityId
    {
        Guid GetGuid();
    }

    public class EntityIdBase<TThis> : ValueOf<Guid, TThis>, IEntityId where TThis : EntityIdBase<TThis>, new()
    {
        public Guid GetGuid() => Value;

        public static TThis New() => new() {Value = Guid.NewGuid()};

        protected override void Validate()
        {
            if (Value == default)
                throw new InvalidIdException("Id must not be default");
        }
    }
}