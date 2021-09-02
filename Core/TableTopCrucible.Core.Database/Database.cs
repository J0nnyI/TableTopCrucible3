using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.Database.Exceptions;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Database.ValueTypes;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Database
{
    public enum DatabaseInitErrorBehavior
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
        void SaveAs(LibraryFilePath file);
        void Close(bool autoSave = true);
        ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDto<Tid, Tentity>;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="behavior">determines what happens if the file has not been closed properly</param>
        void OpenFile(LibraryFilePath file, DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel);
        void New(DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel);
        DatabaseState State { get; }
        IObservable<DatabaseState> StateChanges { get; }

    }
    internal class Database : ReactiveObject, IDatabase
    {
        private readonly ObservableAsPropertyHelper<DatabaseState> _state;
        public DatabaseState State => _state.Value;
        public IObservable<DatabaseState> StateChanges { get; }

        // set once the library is created and ready to be worked with
        [Reactive]
        internal LibraryDirectoryPath LibraryPath { get; private set; }
        [Reactive]
        internal LibraryFilePath CurrentFile { get; private set; }
        internal ConcurrentDictionary<TableName, ITable> tables { get; } = new();

        public Database()
        {
            this.StateChanges = this.WhenAnyValue(vm => vm.LibraryPath)
                .Select(dir => dir != null ? DatabaseState.Open : DatabaseState.Closed);
            this._state = StateChanges
                .ToProperty(this, nameof(State));

            this.New(DatabaseInitErrorBehavior.Override);
        }

        public void Close(bool autoSave = true)
        {
            if (State == DatabaseState.Closed)
                return;
            if (autoSave)
                this.Save();
            this.tables.Values.ToList().ForEach(table => table.Close());
            LibraryPath.Delete();
            this.LibraryPath = null;
        }

        public ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDto<Tid, Tentity>
        {
            var name = TableName.FromType<Tid, Tentity>();
            tables.TryAdd(name, new Table<Tid, Tentity, Tdto>(LibraryPath));
            return tables[name] as ITable<Tid, Tentity, Tdto>;
        }

        public void New(DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            _initialize(null, behavior);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">when true, the old files will be overridden and no exception will be thrown</param>
        /// <param name="force"></param>
        public void OpenFile(LibraryFilePath file, DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            _initialize(file, behavior);
        }
        private void _initialize(LibraryFilePath file, DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            if (State == DatabaseState.Open)
                throw new DatabaseAlreadyOpenedException();

            var dir = file == null
                ? LibraryDirectoryPath.GetTemporaryPath()
                : file.GetWorkingDirectory();
            CurrentFile = file;
            if (dir.Exists())
            {
                switch (behavior)
                {
                    case DatabaseInitErrorBehavior.Cancel:
                        throw new OldDatabaseVersionFoundException();
                    case DatabaseInitErrorBehavior.Override:
                        if (file == null)
                        {
                            dir.Clear();
                        }
                        else
                            file.UnpackLibrary(true);
                        break;
                    case DatabaseInitErrorBehavior.Restore:
                        break;
                    default:
                        throw new InvalidOperationException($"Behavior {behavior} has not been implemented yet");
                }
            }
            else
                dir.Create();

            this.LibraryPath = dir;
        }

        public void Save()
        {
            if (this.CurrentFile == null)
                throw new DatabasenameRequiredException();
            var saveId = TableSaveId.New();
            try
            {
                this.tables.ToList().ForEach(table => table.Value.Save(saveId));
            }
            catch (Exception ex)
            {
                this.tables.ToList().ForEach(table => table.Value.RollBackSave(saveId));

                throw new SaveFailedException(ex);
            }
        }

        public void SaveAs(LibraryFilePath file)
        {
            this.CurrentFile = file;
            var newDir = file.GetWorkingDirectory();
            var oldDir = this.LibraryPath;
            oldDir.Rename(newDir);
            this.LibraryPath = newDir;
            foreach (var table in this.tables.Select(kv => kv.Value))
                table.LibraryDirectory = newDir;
            Save();
        }
    }
}
