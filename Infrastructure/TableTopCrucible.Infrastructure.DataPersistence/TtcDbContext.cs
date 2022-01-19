using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows.Controls.Primitives;

using EntityFrameworkCore.Triggers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic.CompilerServices;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

//dotnet ef migrations add DEV
//dotnet ef migrations remove
namespace TableTopCrucible.Infrastructure.DataPersistence
{
    public class LockInfo : ValueType<string, LockInfo>
    {
        public static LockInfo operator +(LockInfo locker, string add)
            => (LockInfo)(locker.Value + add);

        public static implicit operator LockInfo(string text)
            => From(text);
    }
    public class DeadlockException : Exception
    {
        public DeadlockException(LockInfo lockingTask, LockInfo newTask) : base(
            $"{lockingTask} tried to start {newTask} which would cause a deadlock")
        {

        }
    }
    public class DbContextLockController
    {
        public LockInfo CurrentLock { get; private set; }
        private readonly object _locker = new();
        public T Lock<T>(LockInfo info, Func<T> actionToLock)
        {
            if (CurrentLock is not null)
                throw new DeadlockException(CurrentLock, info);
            T res = default;
            lock (_locker)
            {
                CurrentLock = info;
                res = actionToLock();
                CurrentLock = null;
            }

            return res;
        }
        public void Lock(LockInfo info, Action actionToLock)
        {
            if (CurrentLock is not null)
                throw new DeadlockException(CurrentLock, info);
            lock (_locker)
            {
                CurrentLock = info;
                actionToLock();
                CurrentLock = null;
            }
        }
    }

    public interface IDbSetManager<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>
    {
        IEnumerable<TEntity> Get(LockInfo info, Func<IQueryable<TEntity>, IQueryable<TEntity>> selector);
        TEntity GetSingle(LockInfo info, Func<IQueryable<TEntity>, TEntity> selector);
        TEntity GetSingleOrDefault(LockInfo info, Func<TEntity, bool> selector);
        void Add(LockInfo info, TEntity entity);
        void AddRange(LockInfo info, IEnumerable<TEntity> entities);
        void Remove(LockInfo info, TEntity entity);
        void RemoveRange(LockInfo info, IEnumerable<TEntity> entities);
        int Count(LockInfo info, Func<TEntity, bool> filter = null);
        public bool Any(LockInfo info, Func<TEntity, bool> filter = null);
    }

    public class DbSetManager<TId, TEntity> : IDbSetManager<TId, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>
    {
        internal DbSet<TEntity> _DbSet { get; }
        private readonly DbContextLockController _locker;

        public DbSetManager(DbSet<TEntity> dbSet, DbContextLockController locker)
        {
            _DbSet = dbSet;
            _locker = locker ?? throw new NullReferenceException(nameof(locker));
        }

        public IEnumerable<TEntity> Get(LockInfo info, Func<IQueryable<TEntity>, IQueryable<TEntity>> selector)
            => _locker.Lock(info + nameof(Get), () => selector(_DbSet).ToArray());
        public TEntity GetSingle(LockInfo info, Func<IQueryable<TEntity>, TEntity> selector)
            => _locker.Lock(info + nameof(GetSingle), () => selector(_DbSet));
        public TEntity GetSingleOrDefault(LockInfo info, Func<TEntity, bool> selector)
            => _locker.Lock(info + nameof(GetSingleOrDefault), () => _DbSet.SingleOrDefault(selector));

        public void Add(LockInfo info, TEntity entity)
            => _locker.Lock(info + nameof(Add), () => _DbSet.Add(entity));
        public void AddRange(LockInfo info, IEnumerable<TEntity> entities)
            => _locker.Lock(info + nameof(AddRange), () => _DbSet.AddRange(entities));

        public void Remove(LockInfo info, TEntity entity)
            => _locker.Lock(info + nameof(Remove), () => _DbSet.Remove(entity));
        public void RemoveRange(LockInfo info, IEnumerable<TEntity> entities)
            => _locker.Lock(info + nameof(RemoveRange), () => _DbSet.RemoveRange(entities));

        public int Count(LockInfo info, Func<TEntity, bool> filter = null)
            => _locker.Lock(info + nameof(info), () => _DbSet.Count(filter ?? (_ => true)));
        public bool Any(LockInfo info, Func<TEntity, bool> filter = null)
            => _locker.Lock(info + nameof(info), () => _DbSet.Any(filter ?? (_ => true)));
    }

    public interface IDatabaseContext
    {
        public IDbSetManager<ItemId, Item> Items { get; }
        public IDbSetManager<FileDataId, FileData> Files { get; }
        public IDbSetManager<DirectorySetupId, DirectorySetup> DirectorySetups { get; }
        public IDbSetManager<ImageDataId, ImageData> Images { get; }
        public int SaveChanges();
        public void Migrate();
    }

    public sealed class TtcDbContext : DbContextWithTriggers, IDatabaseContext
    {
        private readonly LibraryFilePath _path;
        private readonly DbContextLockController _locker = new();

        internal TtcDbContext(bool migrate = true, LibraryFilePath path = null)
        {
            _path = path ?? LibraryFilePath.WorkingFile;
            if (migrate)
                Migrate();
        }

        private DbSetManager<ItemId, Item> _items { get; set; }
        private DbSetManager<FileDataId, FileData> _files { get; set; }
        private DbSetManager<DirectorySetupId, DirectorySetup> _directorySetups { get; set; }
        private DbSetManager<ImageDataId, ImageData> _images { get; set; }

        public IDbSetManager<ItemId, Item> Items => _items;
        public IDbSetManager<FileDataId, FileData> Files => _files;
        public IDbSetManager<DirectorySetupId, DirectorySetup> DirectorySetups => _directorySetups;
        public IDbSetManager<ImageDataId, ImageData> Images => _images;

        private DbSet<Item> _itemSet
        {
            get => _items._DbSet;
            set => _items = new(value, _locker);
        }
        private DbSet<FileData> _fileSet
        {
            get => _files._DbSet;
            set => _files = new(value, _locker);
        }
        private DbSet<DirectorySetup> _directorySetupSet
        {
            get => _directorySetups._DbSet;
            set => _directorySetups = new(value, _locker);
        }
        private DbSet<ImageData> _imageSet
        {
            get => _images._DbSet;
            set => _images = new(value, _locker);
        }

        public void Migrate()
        {
            LibraryFilePath.WorkingFile.GetDirectoryPath().Create();
            Database.Migrate();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_path}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("TableTopCrucible.Infrastructure.Models"));


            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
            => _locker.Lock("SaveChanges", base.SaveChanges);
    }
}