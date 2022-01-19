using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models
{
    public class CollectionUpdate<TId, TEntity> 
        where TId : IDataId
        where TEntity :class, IDataEntity<TId>
    {
        public CollectionUpdate(IDbSetManager<TId,TEntity> dbSet, EntityUpdate<TId, TEntity> updateInfo)
        {
            DbSet = dbSet;
            UpdateInfo = updateInfo;
        }

        public IDbSetManager<TId,TEntity> DbSet { get; }
        public EntityUpdate<TId, TEntity> UpdateInfo { get; }
    }
}