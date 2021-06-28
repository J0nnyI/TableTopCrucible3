using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement
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
        void Close();
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
        void Initialize();
        DatabaseState State { get; }

    }
    internal class Database : ReactiveObject, IDatabase
    {
        private ObservableAsPropertyHelper<DatabaseState> _state;
        public DatabaseState State => _state.Value;

        [Reactive]
        internal WorkingDirectoryPath WorkingDirectory { get; private set; }
        [Reactive]
        internal LibraryFilePath CurrentFile { get; private set; }

        public Database()
        {
            this.WhenAnyValue(vm => vm.WorkingDirectory)
                .Select(dir => dir != null ? DatabaseState.Open : DatabaseState.Closed)
                .ToProperty(this, nameof(State));
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public ITable<Tid, Tentity, Tdto> GetTable<Tid, Tentity, Tdto>()
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
            where Tdto : IEntityDTO<Tid, Tentity>
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            WorkingDirectory = WorkingDirectoryPath.GetTemporaryPath();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">when true, the old files will be overridden and no exception will be thrown</param>
        /// <param name="force"></param>
        public void InitializeFromFile(LibraryFilePath file, DatabaseInitializationBehavior behavior = DatabaseInitializationBehavior.Cancel)
        {
            var dir = WorkingDirectoryPath.ForFile(file);
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

            this.WorkingDirectory = dir;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(FilePath file)
        {
            throw new NotImplementedException();
        }
    }
}
