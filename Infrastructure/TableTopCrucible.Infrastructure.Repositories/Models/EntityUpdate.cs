using System.Collections.Generic;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models
{
    public class EntityUpdate<TId, TEntity>
        where TId : IDataId
        where TEntity : IDataEntity<TId>
    {
        public EntityUpdate(EntityUpdateChangeReason changeReason, IReadOnlyDictionary<TId, TEntity> updatedEntities)
        {
            ChangeReason = changeReason;
            UpdatedEntities = updatedEntities;
        }

        public EntityUpdateChangeReason ChangeReason { get; }
        public IReadOnlyDictionary<TId, TEntity> UpdatedEntities { get; }
    }
}