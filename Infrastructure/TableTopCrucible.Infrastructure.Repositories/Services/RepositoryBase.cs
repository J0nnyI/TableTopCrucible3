using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Exceptions;
using TableTopCrucible.Infrastructure.Repositories.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    public interface IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        SourceCache<TEntity, TId> Data { get; }
        TEntity this[TId id] { get; }
        IObservable<TEntity> Watch(TId id);
        IObservable<TEntity> Watch(IObservable<TId> idChanges);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void Remove(TId id);
        void RemoveRange(IEnumerable<TEntity> entities);
        void RemoveRange(IEnumerable<TId> ids);
    }

    internal abstract class RepositoryBase<TId, TEntity>
        : IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        private readonly Subject<EntityUpdate<TId, TEntity>> _changes = new();
        private readonly IStorageController _storageController;

        protected RepositoryBase(IStorageController storageController, SourceCache<TEntity, TId> data)
        {
            _storageController = storageController;
            this.Data = data;
        }

        public SourceCache<TEntity, TId> Data { get; }

        /// <summary>
        /// only updates on init, add and delete, updates have to be consumed via <see cref="INotifyPropertyChanged"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IObservable<TEntity> Watch(TId id)
            => _changes
                .Where(change =>
                    change.UpdatedEntities.ContainsKey(id))
                .Select(change =>
                    change.ChangeReason == EntityUpdateChangeReason.Remove
                        ? null
                        : change.UpdatedEntities[id])
                .StartWith(this[id]);

        /// <summary>
        /// only updates on init, add and delete, updates have to be consumed via <see cref="INotifyPropertyChanged"/>
        /// </summary>
        /// <param name="idChanges"></param>
        /// <returns></returns>
        public IObservable<TEntity> Watch(IObservable<TId> idChanges)
            => idChanges.Select(Watch)
                .Switch()
                .Replay(1)
                .RefCount();

        public TEntity this[TId id]
            => Data.Lookup(id).ToNullable();

        public void Add(TEntity entity)
        {
            Data.Add(entity);
            _storageController.AutoSave();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Data.Add(entities.ToArray());
            _storageController.AutoSave();
        }

        public void Remove(TEntity entity)
        {
            Data.Remove(entity);
            _storageController.AutoSave();
        }

        public void Remove(TId id)
        {
            Remove(this[id]);
            _storageController.AutoSave();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
            => RemoveRange(entities.Select(x => x.Id));

        public void RemoveRange(IEnumerable<TId> ids)
        {
            Data.RemoveKeys(ids);
            _storageController.AutoSave();
        }
    }
}