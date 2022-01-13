using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TableTopCrucible.Core.Helper;
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
        IQueryable<TEntity> Data { get; }
        TEntity this[TId id] { get; }
        IObservable<CollectionUpdate<TId, TEntity>> Updates { get; }
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
        private readonly IDatabaseAccessor _database;
        protected static readonly object _databaseLock = new();

        protected RepositoryBase(IDatabaseAccessor database, DbSet<TEntity> data)
        {
            _database = database;
            _Data = data;


            Updates = _changes.Select(change =>
                    new CollectionUpdate<TId, TEntity>(Data, change))
                .Publish()
                .RefCount()
                .StartWith(
                    new CollectionUpdate<TId, TEntity>(
                        Data,
                        new EntityUpdate<TId, TEntity>(
                            EntityUpdateChangeReason.Init,
                            new Dictionary<TId, TEntity>()
                        )
                    )
                );
        }

        protected DbSet<TEntity> _Data { get; }
        public IQueryable<TEntity> Data => _Data;
        public IObservable<CollectionUpdate<TId, TEntity>> Updates { get; }

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
            => _Data.SingleOrDefault(entity => entity.Guid.Equals(id.Guid));
        public abstract string TableName { get; }

        public void Add(TEntity entity)
        {
            try
            {
                lock (_databaseLock){
                    _Data.Add(entity);
                    _database.AutoSave();
                }
                _changes.OnNext(new EntityUpdate<TId, TEntity>
                    (
                        EntityUpdateChangeReason.Add,
                        new Dictionary<TId, TEntity>
                        {
                            { entity.Id, entity }
                        }
                    )
                );
            }
            catch (DbUpdateException e)
            {
                HandleDbUpdateException(e, entity.AsArray());
                throw;
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                var newEntities = entities.ToArray();
                lock (_databaseLock)
                {

                    _Data.AddRange(newEntities);
                    _database.AutoSave();
                }

                _changes.OnNext(new EntityUpdate<TId, TEntity>
                (
                    EntityUpdateChangeReason.Add,
                    new Dictionary<TId, TEntity>(
                        newEntities.Select(entity => new KeyValuePair<TId, TEntity>(entity.Id, entity)))
                ));
            }
            catch (DbUpdateException e)
            {
                HandleDbUpdateException(e, entities);
                Debugger.Break();
                throw;
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        private void HandleDbUpdateException(DbUpdateException e, IEnumerable<TEntity> entities)
        {
            if (e.InnerException.Message == $"SQLite Error 19: 'UNIQUE constraint failed: {TableName}.Id'.")
                throw new EntityAlreadyAddedException<TId, TEntity>(e, entities);
        }

        public void Remove(TEntity entity)
        {
            lock (_databaseLock)
            {

                _Data.Remove(entity);
                _database.AutoSave();
            }

            _changes.OnNext(
                new EntityUpdate<TId, TEntity>
                (
                    EntityUpdateChangeReason.Remove,
                    new Dictionary<TId, TEntity>
                    {
                        { entity.Id, entity }
                    }
                )
            );
        }

        public void Remove(TId id)
        {
            Remove(this[id]);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                return;
            var entitiesToDelete = entities.ToArray();
            lock (_databaseLock)
            {
                _Data.RemoveRange(entitiesToDelete);
                _database.AutoSave();
            }

            _changes.OnNext(
                new EntityUpdate<TId, TEntity>
                (
                    EntityUpdateChangeReason.Remove,
                    new Dictionary<TId, TEntity>(
                        entitiesToDelete.Distinct().Select(entity => new KeyValuePair<TId, TEntity>(entity.Id, entity)))
                )
            );
        }

        public void RemoveRange(IEnumerable<TId> ids)
            => RemoveRange(Data.Where(entity => ids.Contains(entity.Id)));
    }
}