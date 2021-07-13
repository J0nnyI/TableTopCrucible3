using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Core.DataAccess.ValueTypes;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.DataAccess
{
    public enum DatabaseInitializationBehavior
    {
        // cancels the initialization by throwing an exception
        Cancel,
        Override,
        Restore
    }

    [Singleton(typeof(Database))]
    public interface IDatabase
    {
        void Save();
        void SaveAs(FilePath file);
        void Close(bool autoSave = true);
        ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDTO<Tid, Tentity>;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="behavior">determines what happens if the file has not been closed properly</param>
        void InitializeFromFile(LibraryFilePath file, DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel);
        void Initialize(DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel);
        DatabaseState State { get; }

    }
    internal class Database : ReactiveObject, IDatabase
    {
        private ObservableAsPropertyHelper<DatabaseState> _state;
        public DatabaseState State => _state.Value;

        [Reactive]
        internal LibraryDirectoryPath WorkingDirectory { get; private set; }
        [Reactive]
        internal LibraryFilePath CurrentFile { get; private set; }
        internal ConcurrentDictionary<TableName, ITable> tables { get; } = new ConcurrentDictionary<TableName, ITable>();

        public Database()
        {
            this._state = this.WhenAnyValue(vm => vm.WorkingDirectory)
                .Select(dir => dir != null ? DatabaseState.Open : DatabaseState.Closed)
                .ToProperty(this, nameof(State));
        }

        public void Close(bool autoSave = true)
        {
            if (autoSave)
                this.Save();
            this.tables.Values.ToList().ForEach(table => table.Close(WorkingDirectory));
            WorkingDirectory.Delete();
            this.WorkingDirectory = null;
        }

        public ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDTO<Tid, Tentity>
        {
            var name = TableName.FromType<Tid, Tentity>();
            tables.TryAdd(name, new Table<Tid, Tentity, Tdto>());
            return tables[name] as ITable<Tid, Tentity, Tdto>;
        }

        public void Initialize(DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel)
        {
            _initialize(null, behavior);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">when true, the old files will be overridden and no exception will be thrown</param>
        /// <param name="force"></param>
        public void InitializeFromFile(LibraryFilePath file, DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel)
        {
        }
        private void _initialize(LibraryFilePath file, DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel)
        {
            var dir = file != null
                ? LibraryDirectoryPath.ForFile(file)
                : LibraryDirectoryPath.GetTemporaryPath();
            CurrentFile = file;
            if (dir.Exists())
            {
                switch (behavior)
                {
                    case DatabaseInitializationBehavior.Cancel:
                        throw new OldDatabaseVersionFoundException();
                    case DatabaseInitializationBehavior.Override:
                        file.UnpackLibrary(true);
                        break;
                    case DatabaseInitializationBehavior.Restore:
                        break;
                    default:
                        throw new InvalidOperationException($"Behavior {behavior} has not been implemented yet");
                }
            }
            else
                dir.Create();

            this.WorkingDirectory = dir;
        }

        public void Save()
        {
            var saveId = TableSaveId.New();
            try
            {
                this.tables.ToList().ForEach(table => table.Value.Save(WorkingDirectory,saveId));
            }
            catch (Exception ex)
            {
                this.tables.ToList().ForEach(table => table.Value.RollBackSave(WorkingDirectory, saveId));

                throw new SaveFailedException(ex);
            }
        }

        public void SaveAs(FilePath file)
        {
            throw new NotImplementedException();
        }
    }
}
