using System;
using ValueOf;

namespace TableTopCrucible.Core.DataAccess.Models
{
    public interface IEntityId
    {
        Guid GetGuid();
    }
    public class EntityIdBase<TThis> : ValueOf<Guid, TThis>, IEntityId where TThis : EntityIdBase<TThis>, new()
    {
        public Guid GetGuid()
            => Value;
        public static TThis New()
            => new TThis() { Value = Guid.NewGuid() };
    }
}
