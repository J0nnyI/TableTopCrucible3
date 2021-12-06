using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    enum SaveType
    {
        Auto,
        Manual
    }
    [Singleton]
    public interface IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataEntity> Files { get; }
        public DbSet<DirectorySetupEntity> DirectorySetup { get; }
        void Open(LibraryFilePath file);
        void AutoSave();
    }

    public abstract class DatabaseAccessor :ReactiveObject, IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items => _database.Items;
        public DbSet<ScannedFileDataEntity> Files => _database.Files;
        public DbSet<DirectorySetupEntity> DirectorySetup => _database.DirectorySetups;
        private readonly Subject<SaveType> _onSave = new();
        private IDatabaseContext _database;
        public void Open(LibraryFilePath file)
            => _database = new DatabaseContext(file);

        /// <summary>
        /// handles automated saving
        /// </summary>
        public void AutoSave()
        {
            if(SettingsHelper.AutoCloseEnabled)
                this._onSave.OnNext(SaveType.Auto);
        }

        public void ManualSave()
        {
            this._onSave.OnNext(SaveType.Manual);
        }

        protected DatabaseAccessor()
        {
            // reset buffer on manual save
            _onSave.Where(type=>type == SaveType.Manual)
                .Select(_=>Unit.Default)
                .StartWith(Unit.Default)
                .Select(_=>_onSave
                    .Buffer(SettingsHelper.AutoSaveBuffer))
                .Switch()
                .Subscribe(_ =>_database.SaveChanges());
        }
    }
}
