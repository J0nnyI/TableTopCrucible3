using AutoMapper;

using LiteDB;
using LiteDB.Engine;

using System;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.FileManagement.Exceptions;
using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Database
{
    [Singleton(typeof(DatabaseManager))]
    public interface IDatabaseManager
    {

    }
    public class DatabaseManager:IDatabaseManager, IDisposable
    {
        private LiteDatabase _database;
        protected LiteDatabase database=> _database ?? throw new DatabaseClosedException(); 
        private bool _disposedValue;

        public DatabaseManager()
        {
        }

        public void Open(LibraryFilePath libraryFile)
        {
            if (_database != null)
                throw new DatabaseAlreadyOpenedException();

            this._database = new LiteDatabase(libraryFile.Value);
        }
        public void QuickSave()
        {
            this.database.Checkpoint();
        }
        public void Save()
        {
            this.database.Rebuild();
            this.database.Checkpoint();
        }

        public IDatabaseCollection<Tid, Tentity> GetCollection<Tid, Tentity>()
            where Tid:IEntityId
            where Tentity:IEntity<Tid>
            => new CollectionManager<Tid, Tentity>(_database.GetCollection<Tentity>());

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _database?.Dispose();
                }

                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
