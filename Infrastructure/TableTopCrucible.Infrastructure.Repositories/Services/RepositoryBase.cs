using System;
using System.Collections.Generic;
using System.Linq;

using DynamicData;

using Microsoft.EntityFrameworkCore;

using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    public interface IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        DbSet<TEntity> Data { get; }
    }

    internal abstract class RepositoryBase<TId, TEntity>
        : IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        public DbSet<TEntity> Data { get; }

        protected RepositoryBase(DbSet<TEntity> data)
        {
            Data = data;
        }
    }
}