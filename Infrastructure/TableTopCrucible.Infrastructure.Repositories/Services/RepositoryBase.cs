﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    public interface IRepository<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        SourceCache<TEntity, TId> Data { get; }
        TEntity this[TId id] { get; }
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
        private readonly IStorageController _storageController;

        protected RepositoryBase(IStorageController storageController, SourceCache<TEntity, TId> data)
        {
            _storageController = storageController;
            Data = data;
        }

        public SourceCache<TEntity, TId> Data { get; }
        
        public TEntity this[TId id]
            => Data.Lookup(id).ToNullable();

        public void Add(TEntity entity)
        {
            if (entity is null)
                throw new NullReferenceException(nameof(entity));
            Data.Add(entity);
            _storageController.AutoSave();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities.None())
                return;
            if (entities.Any(x => x is null))
                throw new NullReferenceException("an entity was null");
            Data.Add(entities.ToArray());
            _storageController.AutoSave();
        }

        public void Remove(TEntity entity)
        {
            if (entity is null)
                throw new NullReferenceException(nameof(entity));
            Data.Remove(entity);
            _storageController.AutoSave();
        }

        public void Remove(TId id)
        {
            if (id is null)
                throw new NullReferenceException(nameof(id));
            Remove(this[id]);
            _storageController.AutoSave();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
            => RemoveRange(entities.Select(x => x.Id));

        public void RemoveRange(IEnumerable<TId> ids)
        {
            if (ids.None())
                return;
            if (ids.Any(x => x is null))
                throw new NullReferenceException("an Id was null");
            Data.RemoveKeys(ids);
            _storageController.AutoSave();
        }
    }
}