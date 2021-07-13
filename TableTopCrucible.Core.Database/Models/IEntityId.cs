using System;
using System.Collections.Generic;
using System.Text;

using ValueOf;

namespace TableTopCrucible.Core.FileManagement.Models
{
    public interface IEntityId
    {
        public Guid GetGuid();
    }
    public class EntityIdBase<Tthis> : ValueOf<Guid, Tthis>, IEntityId where Tthis : EntityIdBase<Tthis>, new()
    {
        public Guid GetGuid()
            => Value;
        public static Tthis New()
            => new Tthis() { Value = Guid.NewGuid() };
    }
}
