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
        void Update(TChangeSet newModel);
        void Update(IEnumerable<TChangeSet> newModels);
        void Add(TChangeSet newModel);
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

        public void Add(TChangeSet newModel)
        {
            _dbSet.Add(newModel.ToEntity());
            _cache.AddOrUpdate(newModel.ToModel());
            _database.AutoSave();
        }
        public void Update(TChangeSet newModel)
        {
            _dbSet.Update(newModel.ToEntity());
            _cache.AddOrUpdate(newModel.ToModel());
            _database.AutoSave();
        }

        public void Update(IEnumerable<TChangeSet> newModels)
        {
            var modelArray = newModels.ToArray();
            var entities = modelArray.Select(changeSet=>changeSet.ToEntity()).ToArray();
            _dbSet.UpdateRange(entities);
            var models = modelArray.Select(changeSet => changeSet.ToModel()).ToArray();
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
            var guidArray = idArray.Select(id => id.Guid).ToArray();
            var entities = _dbSet.Where(entity=>guidArray.Contains(entity.Id));
            _dbSet.RemoveRange(entities);
            _cache.RemoveKeys(idArray);
            _database.AutoSave();
        }

        public IObservable<Change<TModel, TId>> Watch(TId id)
            => _cache.Watch(id);
    }
}