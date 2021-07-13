using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using AutoMapper;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Core.DataAccess.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.DataAccess
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
        DateTime? LastChange { get; }
        LibraryDirectoryPath WorkingDirectory { get; }

        void Save(LibraryDirectoryPath workingDirectory, TableSaveId saveId);
        void RollBackSave(LibraryDirectoryPath workingDirectory, TableSaveId saveId);
        internal void Close(LibraryDirectoryPath workingDirectory);
    }


    public interface ITable<Tid, Tentity, Tdto> : ITable
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDTO<Tid, Tentity>
    {

        void AddOrUpdate(Tentity entity);
        IObservable<Tentity> WatchValue(Tid entityId);
    }


    internal abstract class Table : ReactiveObject, ITable
    {

        public abstract TableName Name { get; }
        [Reactive]
        public LibraryDirectoryPath WorkingDirectory { get; }
        [Reactive]
        public DatabaseState State { get; protected set; }
        [Reactive]
        public DateTime? LastSave { get; protected set; }
        [Reactive]
        public DateTime? LastChange { get; protected set; }
        protected readonly IMapper _mapper;

        public Table(LibraryDirectoryPath workingDirectory)
        {
            _mapper = Locator.Current.GetService<IMapper>();
            this.WorkingDirectory = workingDirectory;
        }

        public abstract void RollBackSave(LibraryDirectoryPath workingDirectory, TableSaveId saveId);

        public abstract void Save(LibraryDirectoryPath workingDirectory, TableSaveId saveId);
        internal abstract void Close(LibraryDirectoryPath workingDirectory);

        void ITable.Close(LibraryDirectoryPath workingDirectory)
            => Close(workingDirectory);
    }


    internal class Table<Tid, Tentity, Tdto> : Table, ITable<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDTO<Tid, Tentity>
    {
        private readonly SourceCache<Tentity, Tid> _data
             = new SourceCache<Tentity, Tid>(data => data.Id);

        public override TableName Name 
            => TableName.FromType<Tid, Tentity>();

        public Table() : base(Locator.Current.GetService<IMapper>())
        {
        }

        public void AddOrUpdate(Tentity entity)
        {
            this._data.AddOrUpdate(entity);
            LastChange = DateTime.Now;
        }

        public IObservable<Tentity> WatchValue(Tid entityId)
            => this._data.WatchValue(entityId);
        public IObservable<Tentity> WatchValue(IObservable<Tid> entityIdChanges)
            => entityIdChanges.Select(entityId => this._data.WatchValue(entityId)).Switch();

        public override void Save(LibraryDirectoryPath workingDirectory, TableSaveId saveId)
        {
            if (workingDirectory is null)
                throw new ArgumentNullException(nameof(workingDirectory));
            if (saveId is null)
                throw new ArgumentNullException(nameof(saveId));

            var file = TableFilePath.From(workingDirectory, saveId, TableName.FromType<Tid, Tentity>());
            try
            {
                var dto = _mapper.Map<IEnumerable<Tdto>>(this._data.Items);
                file.WriteObject(dto);
            }
            catch (Exception ex) when (
                ex is FileWriteFailedException
                || ex is SerializationFailedException)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DtoConversionException<Tentity, Tdto>(ex);
            }
        }

        public override void RollBackSave(LibraryDirectoryPath workingDirectory, TableSaveId saveId)
        {
            if (workingDirectory is null)
                throw new ArgumentNullException(nameof(workingDirectory));
            if (saveId is null)
                throw new ArgumentNullException(nameof(saveId));

            var file = TableFilePath.From(workingDirectory, saveId, TableName.FromType<Tid, Tentity>());
            file.TryDelete();
        }

        internal override void Close(LibraryDirectoryPath workingDirectory)
        {
            if (workingDirectory is null)
                throw new ArgumentNullException(nameof(workingDirectory));
            
            (workingDirectory + Name.GetRelativePath()).Delete();
            this._data.Dispose();
            this.State = DatabaseState.Closed;
        }
    }
}
