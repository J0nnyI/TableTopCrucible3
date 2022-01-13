using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models
{
    public class CollectionUpdate<TId, TEntity> : IQueryable<TEntity>
        where TId : IDataId
        where TEntity : IDataEntity<TId>
    {
        public CollectionUpdate(IQueryable<TEntity> queryable, EntityUpdate<TId, TEntity> updateInfo)
        {
            Queryable = queryable;
            UpdateInfo = updateInfo;
        }

        public IQueryable<TEntity> Queryable { get; }
        public EntityUpdate<TId, TEntity> UpdateInfo { get; }

        #region Queryable

        public IEnumerator<TEntity> GetEnumerator() => Queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Queryable).GetEnumerator();

        public Type ElementType => Queryable.ElementType;

        public Expression Expression => Queryable.Expression;

        public IQueryProvider Provider => Queryable.Provider;

        #endregion
    }
}