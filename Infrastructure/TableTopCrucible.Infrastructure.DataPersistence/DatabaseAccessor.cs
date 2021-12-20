using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    internal enum SaveType
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

        //void Open(LibraryFilePath file);
        void AutoSave();
        void ManualSave();
    }

    public class DatabaseAccessor : ReactiveObject, IDatabaseAccessor
    {
        private readonly Subject<SaveType> _onSave = new();
        private IDatabaseContext _database;


        public DatabaseAccessor()
        {
            var file = (LibraryFilePath)SettingsHelper.DefaultFilePath;


            Open(file);
            // reset buffer on manual save
            _onSave.Where(type => type == SaveType.Manual)
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default)
                .Select(_ => _onSave
                    .Buffer(SettingsHelper.AutoSaveBuffer))
                .Switch()
                .Subscribe(_ => _database.SaveChanges());
        }

        public DbSet<ItemEntity> Items => _database.Items;
        public DbSet<ScannedFileDataEntity> Files => _database.Files;
        public DbSet<DirectorySetupEntity> DirectorySetup => _database.DirectorySetups;

        /// <summary>
        ///     handles automated saving
        /// </summary>
        public void AutoSave()
        {
            if (SettingsHelper.AutoSaveEnabled)
                _database.SaveChanges();
            //this._onSave.OnNext(SaveType.Auto);
        }

        public void ManualSave()
        {
            try
            {
                //this._onSave.OnNext(SaveType.Manual);
                _database.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Open(LibraryFilePath file)
        {
            if (!file.IsWorkingFile)
                file.Copy(LibraryFilePath.WorkingFile);

            _database = new DatabaseContext();
        }
    }
}