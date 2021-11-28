using System;

namespace TableTopCrucible.Core.Database.Models
{
    public interface IEntityDto<Tid, Tentity>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
    {
        Guid Id { get; set; }
        Tentity ToEntity();
        void Initialize(Tentity sourceEntity);
    }
}