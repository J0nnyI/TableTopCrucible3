using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

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
        public IDbSetManager<ItemId, Item> Items { get; }
        public IDbSetManager<FileDataId, FileData> Files { get; }
        public IDbSetManager<ImageDataId, ImageData> Images { get; }
        public IDbSetManager<DirectorySetupId, DirectorySetup> DirectorySetup { get; }

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

        public IDbSetManager<ItemId, Item> Items => _database.Items;
        public IDbSetManager<FileDataId, FileData> Files => _database.Files;
        public IDbSetManager<DirectorySetupId, DirectorySetup> DirectorySetup => _database.DirectorySetups;
        public IDbSetManager<ImageDataId, ImageData> Images => _database.Images;

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

            _database = new TtcDbContext(false);
            _database.Migrate();
        }
    }
}