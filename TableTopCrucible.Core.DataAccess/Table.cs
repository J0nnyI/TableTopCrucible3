using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

using AutoMapper;

using TableTopCrucible.Core.DataAccess.ValueTypes;
using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;
using Splat;
using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using System.Reactive.Linq;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace TableTopCrucible.Core.FileManagement
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
        void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId);
        void RollBackSave(WorkingDirectoryPath workingDirectory, TableSaveId saveId);
        internal void Close(WorkingDirectoryPath workingDirectory);
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
        public DatabaseState State { get; protected set; }
        [Reactive]
        public DateTime? LastSave { get; protected set; }
        [Reactive]
        public DateTime? LastChange { get; protected set; }
        protected readonly IMapper _mapper;

        public Table(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public abstract void RollBackSave(WorkingDirectoryPath workingDirectory, TableSaveId saveId);

        public abstract void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId);
        internal abstract void Close(WorkingDirectoryPath workingDirectory);

        void ITable.Close(WorkingDirectoryPath workingDirectory)
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

        public override void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId)
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

        public override void RollBackSave(WorkingDirectoryPath workingDirectory, TableSaveId saveId)
        {
            if (workingDirectory is null)
                throw new ArgumentNullException(nameof(workingDirectory));
            if (saveId is null)
                throw new ArgumentNullException(nameof(saveId));

            var file = TableFilePath.From(workingDirectory, saveId, TableName.FromType<Tid, Tentity>());
            file.TryDelete();
        }

        internal override void Close(WorkingDirectoryPath workingDirectory)
        {
            if (workingDirectory is null)
                throw new ArgumentNullException(nameof(workingDirectory));
            
            (workingDirectory + Name.GetRelativePath()).Delete();
            this._data.Dispose();
            this.State = DatabaseState.Closed;
        }
    }
}
