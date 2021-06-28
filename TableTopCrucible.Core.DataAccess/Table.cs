using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;

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
        [Reactive]
        public TableName Name { get; set; }
        [Reactive]
        public DatabaseState State { get; protected set; }
        [Reactive]
        public DateTime? LastSave { get; protected set; }
        [Reactive]
        public DateTime? LastChange { get; protected set; }
    }


    internal class Table<Tid, Tentity, Tdto> : Table, ITable<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDTO<Tid, Tentity>
    {
        private readonly SourceCache<Tentity, Tid> _data
             = new SourceCache<Tentity, Tid>(data => data.Id);


        public void AddOrUpdate(Tentity entity)
        {
            throw new NotImplementedException();
        }

        public IObservable<Tentity> WatchValue(Tid entityId)
        {
            throw new NotImplementedException();
        }
    }
}
