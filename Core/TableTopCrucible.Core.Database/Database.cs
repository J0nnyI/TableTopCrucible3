using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.Database.Exceptions;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Database
{
    public enum DatabaseInitErrorBehavior
    {
        // cancels the initialization by throwing an exception
        Cancel,
        Override,
        Restore
    }

    [Singleton]
    public interface IDatabase
    {
        DatabaseState State { get; }
        IObservable<DatabaseState> StateChanges { get; }
        void Save();
        void SaveAs(LibraryFilePath file);
        void Close(bool autoSave = true);

        ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDto<Tid, Tentity>;

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="behavior">determines what happens if the file has not been closed properly</param>
        void OpenFile(LibraryFilePath file, DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel);

        void New(DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel);
    }

    public class Database : ReactiveObject, IDatabase
    {
        private readonly ObservableAsPropertyHelper<DatabaseState> _state;

        public Database()
        {
            StateChanges = this.WhenAnyValue(vm => vm.LibraryPath)
                .Select(dir => dir != null ? DatabaseState.Open : DatabaseState.Closed);
            _state = StateChanges
                .ToProperty(this, nameof(State));

            New(DatabaseInitErrorBehavior.Override);
        }

        // set once the library is created and ready to be worked with
        [Reactive] internal LibraryDirectoryPath LibraryPath { get; private set; }

        [Reactive] internal LibraryFilePath CurrentFile { get; private set; }

        internal ConcurrentDictionary<TableName, ITable> _Tables { get; } = new();
        public DatabaseState State => _state.Value;
        public IObservable<DatabaseState> StateChanges { get; }

        public void Close(bool autoSave = true)
        {
            if (State == DatabaseState.Closed)
                return;
            if (autoSave)
                Save();
            _Tables.Values.ToList().ForEach(table => table.Close());
            LibraryPath.Delete();
            LibraryPath = null;
        }

        public ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDto<Tid, Tentity>
        {
            var name = TableName.FromType<Tid, Tentity>();
            if (!_Tables.ContainsKey(name))
                _Tables.TryAdd(name, new Table<Tid, Tentity, Tdto>(LibraryPath));
            return _Tables[name] as ITable<Tid, Tentity, Tdto>;
        }

        public void New(DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            _initialize(null, behavior);
        }

        /// <summary>
        /// </summary>
        /// <param name="file">when true, the old files will be overridden and no exception will be thrown</param>
        /// <param name="behavior"></param>
        public void OpenFile(LibraryFilePath file,
            DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            _initialize(file, behavior);
        }

        public void Save()
        {
            if (CurrentFile == null)
                throw new DatabaseNameRequiredException();
            var saveId = TableSaveId.New();
            try
            {
                _Tables.ToList().ForEach(table => table.Value.Save(saveId));
            }
            catch (Exception ex)
            {
                _Tables.ToList().ForEach(table => table.Value.RollBackSave(saveId));

                throw new SaveFailedException(ex);
            }
        }

        public void SaveAs(LibraryFilePath file)
        {
            CurrentFile = file;
            var newDir = file.GetWorkingDirectory();
            var oldDir = LibraryPath;
            oldDir.Rename(newDir);
            LibraryPath = newDir;
            foreach (var table in _Tables.Select(kv => kv.Value))
                table.LibraryDirectory = newDir;
            Save();
        }

        private void _initialize(LibraryFilePath file,
            DatabaseInitErrorBehavior behavior = DatabaseInitErrorBehavior.Cancel)
        {
            if (State == DatabaseState.Open)
                throw new DatabaseAlreadyOpenedException();

            var dir = file == null
                ? LibraryDirectoryPath.GetTemporaryPath()
                : file.GetWorkingDirectory();

            CurrentFile = file;
            if (dir.Exists())
                switch (behavior)
                {
                    case DatabaseInitErrorBehavior.Cancel:
                        throw new OldDatabaseVersionFoundException();
                    case DatabaseInitErrorBehavior.Override:
                        if (file == null)
                            dir.Clear();
                        else
                            file.UnpackLibrary(true);
                        break;
                    case DatabaseInitErrorBehavior.Restore:
                        break;
                    default:
                        throw new InvalidOperationException($"Behavior {behavior} has not been implemented yet");
                }
            else
                dir.Create();

            LibraryPath = dir;
        }
    }
}