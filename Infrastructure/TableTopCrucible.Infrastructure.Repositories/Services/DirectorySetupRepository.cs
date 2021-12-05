using System;
using System.Collections.Generic;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.ChangeSets;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    internal abstract class RepositoryBase<TModel, TId, TChangeSet, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity, new()
        where TModel : class, IDataModel<TId>, new()
        where TChangeSet: class, IDataChangeSet<TId, TEntity, TModel>, new()
    {
        public IObservableCache<TModel, TId> Cache => _cache;
        private readonly SourceCache<TModel, TId> _cache = new(e => e.Id);
        private readonly DbSet<TEntity> _dbSet;

        protected RepositoryBase(DbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }
        public IObservable<IEnumerable<TModel>> TakenDirectoriesChanges { get; }

        public void Add(TChangeSet newModel)
        {
            _dbSet.Update(newModel.ToEntity());
            _cache.AddOrUpdate(newModel.ToModel());
        }

    }

    [Singleton]
    public interface IDirectorySetupRepository
    {
        IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }
        IObservableCache<DirectorySetupModel, DirectorySetupId> Cache { get; }
    }
    internal class DirectorySetupRepository : IDirectorySetupRepository
    {
        public IObservableCache<DirectorySetupModel, DirectorySetupId> Cache => _cache;
        private readonly SourceCache<DirectorySetupModel, DirectorySetupId> _cache = new(e => e.Id);
        private readonly IDatabaseAccessor _database;

        public DirectorySetupRepository(IDatabaseAccessor database)
        {
            _database = database;
        }
        public IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }

        public void Add(DirectorySetupChangeSet newModel)
        {
            _database.DirectorySetup.Update(newModel.ToEntity());
            _cache.AddOrUpdate(newModel.ToModel());
        }
    }
}
