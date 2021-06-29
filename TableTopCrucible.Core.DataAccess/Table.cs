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

namespace TableTopCrucible.Core.FileManagement
{
    public enum DatabaseState
    {
        Open,
        Closed
    }


    public interface ITable : IReactiveObject
    {
        TableName Name { get; set; }
        DatabaseState State { get; }
        DateTime? LastSave { get; }
        DateTime? LastChange { get; }
        void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId);
        void RollBack(TableSaveId saveName);
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
        protected readonly Mapper _mapper;

        [Reactive]
        public TableName Name { get; set; }
        [Reactive]
        public DatabaseState State { get; protected set; }
        [Reactive]
        public DateTime? LastSave { get; protected set; }
        [Reactive]
        public DateTime? LastChange { get; protected set; }

        public Table(Mapper mapper)
        {
            _mapper = mapper;
        }

        public void RollBack(TableSaveId saveName)
        {
            throw new NotImplementedException();
        }

        public abstract void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId);
    }


    internal class Table<Tid, Tentity, Tdto> : Table, ITable<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDTO<Tid, Tentity>
    {
        private readonly SourceCache<Tentity, Tid> _data
             = new SourceCache<Tentity, Tid>(data => data.Id);

        public Table() : base(Locator.Current.GetService<Mapper>())
        {
        }

        public void AddOrUpdate(Tentity entity)
        {
            throw new NotImplementedException();
        }

        public IObservable<Tentity> WatchValue(Tid entityId)
        {
            throw new NotImplementedException();
        }


        public override void Save(WorkingDirectoryPath workingDirectory, TableSaveId saveId)
        {
            var file = TableFilePath.From(workingDirectory, saveId, TableName.FromType<Tid, Tentity>());
            try
            {
                var dto = _mapper.Map<Tdto>(this._data.Items);
                file.WriteObject(dto);
            }
            catch (Exception ex) when (
                ex is FileWriteFailedException 
                || ex is SerializationFailedException) {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DtoConversionException(ex);
            }
        }
    }
}
