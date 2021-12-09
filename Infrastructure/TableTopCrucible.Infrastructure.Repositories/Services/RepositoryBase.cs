using System;
using System.Collections.Generic;
using System.Linq;

using DynamicData;

using Microsoft.EntityFrameworkCore;

using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.ChangeSets;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    public interface IRepository<TId, TModel, TEntity, TChangeSet>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel>, new()
        where TModel : class, IDataModel<TId>
        where TChangeSet : class, IDataChangeSet<TId, TModel, TEntity>
    {
        IObservableCache<TModel, TId> Cache { get; }
        IEnumerable<KeyValuePair<TId, TModel>> KeyValues { get; }
        IEnumerable<TModel> Values { get; }

        void Remove(TId id);
        void Remove(IEnumerable<TId> ids);
        void Update(TChangeSet changeSet);
        void Update(IEnumerable<TChangeSet> newChangeSets);
        void Add(TChangeSet newData);
        void Add(IEnumerable<TChangeSet> newData);
        public IObservable<Change<TModel, TId>> Watch(TId id);
    }

    internal abstract class RepositoryBase<TId, TModel, TEntity, TChangeSet>
        : IRepository<TId, TModel, TEntity, TChangeSet>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel>, new()
        where TModel : class, IDataModel<TId>
        where TChangeSet : class, IDataChangeSet<TId, TModel, TEntity>
    {
        public IObservableCache<TModel, TId> Cache => _cache.AsObservableCache();
        public IEnumerable<KeyValuePair<TId, TModel>> KeyValues => _cache.KeyValues;
        public IEnumerable<TModel> Values => _cache.Items;
        protected readonly SourceCache<TModel, TId> _cache = new(e => e.Id);
        private readonly IDatabaseAccessor _database;
        protected readonly DbSet<TEntity> _dbSet;

        protected RepositoryBase(IDatabaseAccessor database, DbSet<TEntity> dbSet)
        {
            _database = database;
            _dbSet = dbSet;

            _cache.AddOrUpdate(_dbSet.Select(entity => entity.ToModel()));
        }

        public void Add(TChangeSet newData)
        {
            _dbSet.Add(newData.ToEntity());
            _cache.AddOrUpdate(newData.ToModel());
            _database.AutoSave();
        }
        public void Add(IEnumerable<TChangeSet> newData)
        {
            _dbSet.AddRange(newData.Select(data=>data.ToEntity()));
            _cache.AddOrUpdate(newData.Select(data => data.ToModel()));
            _database.AutoSave();
        }
        public void Update(TChangeSet changeSet)
        {
            var entity = _dbSet.Single(dbEntity => dbEntity.Id == changeSet.Id.Guid);
            changeSet.UpdateEntity(entity);
            _dbSet.Update(entity);
            _cache.AddOrUpdate(changeSet.ToModel());
            _database.AutoSave();
        }

        public void AddOrUpdate(IEnumerable<TChangeSet> newChangeSets)
        {
            var changeSetArray = newChangeSets.ToArray();

            var entities = changeSetArray
                .Select(changeSet =>
                {
                    var entity = _dbSet.Single(dbEntity => dbEntity.Id == changeSet.Id.Guid);
                    changeSet.UpdateEntity(entity);
                    return entity;
                })
                .ToArray();
            _dbSet.UpdateRange(entities);

            var models = changeSetArray.Select(changeSet => changeSet.ToModel()).ToArray();
            _cache.AddOrUpdate(models);
            _database.AutoSave();
        }

        public void Update(IEnumerable<TChangeSet> newChangeSets)
        {
            var changeSetArray = newChangeSets.ToArray();

            var entities = changeSetArray
                .Select(changeSet =>
                {
                    var entity = _dbSet.Single(dbEntity => dbEntity.Id == changeSet.Id.Guid);
                    changeSet.UpdateEntity(entity);
                    return entity;
                })
                .ToArray();
            _dbSet.UpdateRange(entities);

            var models = changeSetArray.Select(changeSet => changeSet.ToModel()).ToArray();
            _cache.AddOrUpdate(models);
            _database.AutoSave();
        }

        public void Remove(TId id)
        {
            try
            {
                var original = _dbSet.Single(e => e.Id == id.Guid);
                _dbSet.Remove(original);
                _cache.RemoveKey(id);
                _database.AutoSave();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Remove(IEnumerable<TId> ids)
        {
            var idArray = ids.ToArray();

            if(idArray.Length==0)
                return;

            var guidArray = idArray.Select(id => id.Guid).ToArray();
            var entities = _dbSet.Where(entity => guidArray.Contains(entity.Id));
            _dbSet.RemoveRange(entities);
            _cache.RemoveKeys(idArray);
            _database.AutoSave();
        }

        public IObservable<Change<TModel, TId>> Watch(TId id)
            => _cache.Watch(id);
    }
}