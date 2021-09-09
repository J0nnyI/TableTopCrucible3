using AutoMapper;

using DynamicData;
using DynamicData.Kernel;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

using TableTopCrucible.Core.Database.Exceptions;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.Database
{
    public enum DatabaseState
    {
        Open,
        Closed
    }


    public interface ITable : IReactiveObject
    {
        TableName Name { get; }
        DatabaseState State { get; }
        DateTime? LastSave { get; }
        IObservable<Optional<DateTime>> LastUpdateChanges { get; }
        LibraryDirectoryPath LibraryDirectory { get; internal set; }

        void Save(TableSaveId saveId);
        void RollBackSave(TableSaveId saveId);
        int Count { get; }
        internal void Close();
    }


    public interface ITable<Tid, Tentity, Tdto> : ITable
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {

        void AddOrUpdate(Tentity entity);
        void AddOrUpdate(IEnumerable<Tentity> entity);

        IObservable<Tentity> WatchValue(Tid entityId);
        IConnectableCache<Tentity, Tid> DataChanges { get; }
        IObservableCache<Tentity, Tid> Data { get; }
        public void Remove(Tid id);
    }


    internal abstract class Table : ReactiveObject, ITable
    {
        public abstract TableName Name { get; }
        protected abstract IObservable<Unit> OnDataUpdate { get; }
        [Reactive]
        public LibraryDirectoryPath LibraryDirectory { get; protected set; }
        LibraryDirectoryPath ITable.LibraryDirectory
        {
            get => this.LibraryDirectory;
            set => this.LibraryDirectory = value;
        }
        [Reactive]
        public DatabaseState State { get; protected set; }
        [Reactive]
        public DateTime? LastSave { get; protected set; }
        public IObservable<Optional<DateTime>> LastUpdateChanges { get; }
        protected readonly IMapper mapper;

        protected Table(LibraryDirectoryPath libraryDirectory)
        {
            mapper = Locator.Current.GetService<IMapper>();
            this.LibraryDirectory = libraryDirectory;

            this.LastUpdateChanges = this.OnDataUpdate
                .Select(_ => Optional.Some(DateTime.Now))
                .StartWith(Optional.None<DateTime>())
                .Replay(1);
        }

        public abstract void RollBackSave(TableSaveId saveId);

        public abstract void Save(TableSaveId saveId);
        internal abstract void Close();

        void ITable.Close()
            => Close();

        public abstract int Count { get; }

    }


    internal class Table<Tid, Tentity, Tdto>
        : Table, ITable<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        private readonly SourceCache<Tentity, Tid> _data = new(data => data.Id);

        public override TableName Name
            => TableName.FromType<Tid, Tentity>();

        protected override IObservable<Unit> OnDataUpdate => _data.Connect().Select(_ => Unit.Default);
        public IConnectableCache<Tentity, Tid> DataChanges => _data;
        public IObservableCache<Tentity, Tid> Data => _data;


        public Table(LibraryDirectoryPath libraryDirectory) : base(libraryDirectory)
        {
            if (LibraryDirectory is null)
                throw new ArgumentNullException(nameof(LibraryDirectory));

        }

        public void AddOrUpdate(Tentity entity)
        {
            this._data.AddOrUpdate(entity);
        }
        public void AddOrUpdate(IEnumerable<Tentity> entity)
            => this._data.AddOrUpdate(entity);


        public void Remove(Tid id)
            => _data.Remove(id);

        public override int Count => _data.Count;


        public IObservable<Tentity> WatchValue(Tid entityId)
            => this._data.WatchValue(entityId);
        public IObservable<Tentity> WatchValue(IObservable<Tid> entityIdChanges)
            => entityIdChanges.Select(entityId => this._data.WatchValue(entityId)).Switch();

        public override void Save(TableSaveId saveId)
        {
            if (saveId is null)
                throw new ArgumentNullException(nameof(saveId));

            var file = _getFilepath(saveId);
            try
            {
                var dto = mapper.Map<IEnumerable<Tdto>>(this._data.Items);
                file.WriteObject(dto);
            }
            catch (Exception ex) when (
                ex is FileWriteFailedException ||
                ex is SerializationFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DtoConversionException<Tentity, Tdto>(ex);
            }
        }

        public override void RollBackSave(TableSaveId saveId)
        {
            if (saveId is null)
                throw new ArgumentNullException(nameof(saveId));

            var file = _getFilepath(saveId);
            file.TryDelete();
        }

        private TableFilePath _getFilepath(TableSaveId saveId) => TableFilePath.From(LibraryDirectory, saveId, TableName.FromType<Tid, Tentity>());

        internal override void Close()
        {
            (LibraryDirectory + Name.GetRelativePath()).Delete();
            this._data.Dispose();
            this.State = DatabaseState.Closed;
        }
    }
}
