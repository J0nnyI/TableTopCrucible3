using System;
using ValueOf;

namespace TableTopCrucible.Core.DataAccess.Models
{
    public interface IEntityId
    {
        Guid GetGuid();
    }
    public class EntityIdBase<Tthis> : ValueOf<Guid, Tthis>, IEntityId where Tthis : EntityIdBase<Tthis>, new()
    {
        public Guid GetGuid()
            => Value;
        public static Tthis New()
            => new Tthis() { Value = Guid.NewGuid() };
    }
}
