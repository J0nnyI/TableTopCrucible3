using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Navigation;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    public enum EntityUpdateChangeReason
    {
        Add,
        Remove,
        Init
    }

    public static class CollectionUpdateHelper
    {
        public static IObservable<IChangeSet<TEntity, TId>> ToObservableCache<TId, TEntity>(
            this IObservable<CollectionUpdate<TId, TEntity>> updateSource,
            CompositeDisposable disposeWith
        )
            where TId : IDataId
            where TEntity : class, IDataEntity<TId>, new()
        {
            return updateSource
                .Scan(
                    new SourceCache<TEntity, TId>(entity => entity.Id)
                        .DisposeWith(disposeWith),
                    (list, change) =>
                    {
                        switch (change.UpdateInfo.ChangeReason)
                        {
                            case EntityUpdateChangeReason.Add:
                                list.AddOrUpdate(change.UpdateInfo.UpdatedEntities.Values);
                                break;
                            case EntityUpdateChangeReason.Remove:
                                list.Remove(change.UpdateInfo.UpdatedEntities.Values);
                                break;
                            case EntityUpdateChangeReason.Init:
                                list.AddOrUpdate(change.Queryable.AsEnumerable());
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        return list;
                    })
                .Select(cache => cache.Connect())
                .Switch();
        }
    }

    public class EntityUpdate<TId, TEntity>
        where TId : IDataId
        where TEntity : IDataEntity<TId>
    {
        public EntityUpdateChangeReason ChangeReason { get; }
        public IReadOnlyDictionary<TId, TEntity> UpdatedEntities { get; }

        public EntityUpdate(EntityUpdateChangeReason changeReason, IReadOnlyDictionary<TId, TEntity> updatedEntities)
        {
            ChangeReason = changeReason;
            UpdatedEntities = updatedEntities;
        }
    }

    public class CollectionUpdate<TId, TEntity> : IQueryable<TEntity>
        where TId : IDataId
        where TEntity : IDataEntity<TId>
    {
        public IQueryable<TEntity> Queryable { get; }
        public EntityUpdate<TId, TEntity> UpdateInfo { get; }

        public CollectionUpdate(IQueryable<TEntity> queryable, EntityUpdate<TId, TEntity> updateInfo)
        {
            Queryable = queryable;
            UpdateInfo = updateInfo;
        }

        #region Queryable

        public IEnumerator<TEntity> GetEnumerator() => Queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Queryable).GetEnumerator();

        public Type ElementType => Queryable.ElementType;

        public Expression Expression => Queryable.Expression;

        public IQueryProvider Provider => Queryable.Provider;

        #endregion
    }

    public interface IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        IQueryable<TEntity> Data { get; }
        IObservable<TEntity> Watch(TId id);
        IObservable<TEntity> Watch(IObservable<TId> idChanges);
        TEntity this[TId id] { get; }
        IObservable<CollectionUpdate<TId, TEntity>> Updates { get; }
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
        private readonly IDatabaseAccessor _database;
        protected DbSet<TEntity> _Data { get; }
        public IQueryable<TEntity> Data => _Data;
        private readonly Subject<EntityUpdate<TId, TEntity>> _changes = new();
        public IObservable<CollectionUpdate<TId, TEntity>> Updates { get; }

        public IObservable<TEntity> Watch(TId id)
            => _changes
                .Where(change =>
                    change.UpdatedEntities.ContainsKey(id))
                .Select(change =>
                    change.UpdatedEntities[id])
                .StartWith(Data.SingleOrDefault(entity => entity.Id.Equals(id)));

        public IObservable<TEntity> Watch(IObservable<TId> idChanges)
            => idChanges.Select(Watch)
                .Switch()
                .Replay(1)
                .RefCount();

        public TEntity this[TId id]
            => _Data.Single(entity => entity.Id.Equals(id));

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

        public void Add(TEntity entity)
        {
            _Data.Add(entity);
            _database.AutoSave();
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

        public void AddRange(IEnumerable<TEntity> entities)
        {
            var newEntities = entities.ToArray();
            _Data.AddRange(newEntities);
            _database.AutoSave();
            _changes.OnNext(new EntityUpdate<TId, TEntity>
            (
                EntityUpdateChangeReason.Add,
                new Dictionary<TId, TEntity>(
                    newEntities.Select(entity => new KeyValuePair<TId, TEntity>(entity.Id, entity)))
            ));
        }

        public void Remove(TEntity entity)
        {
            _Data.Remove(entity);
            _database.AutoSave();
            _changes.OnNext(
                new EntityUpdate<TId, TEntity>
                (
                    EntityUpdateChangeReason.Remove,
                    new Dictionary<TId, TEntity>
                    {
                        { entity.Id, null }
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
            var entitiesToDelete = entities.ToArray();
            _Data.RemoveRange(entitiesToDelete);
            _database.AutoSave();
            _changes.OnNext(
                new EntityUpdate<TId, TEntity>
                (
                    EntityUpdateChangeReason.Remove,
                    new Dictionary<TId, TEntity>(
                        entitiesToDelete.Select(entity => new KeyValuePair<TId, TEntity>(entity.Id, entity)))
                )
            );
        }

        public void RemoveRange(IEnumerable<TId> ids)
            => RemoveRange(Data.Where(entity => ids.Contains(entity.Id)));
    }
}