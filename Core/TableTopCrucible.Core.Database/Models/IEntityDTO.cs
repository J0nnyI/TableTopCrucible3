using System;

namespace TableTopCrucible.Core.Database.Models
{
    public interface IEntityDto<Tid, Tentity>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
    {
        public Guid IdValue { get; set; }
    }
}
