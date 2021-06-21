using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.FileManagement.ValueTypes;

namespace TableTopCrucible.Core.FileManagement
{
    public interface IDataCrate
    {
        DataCrateName Name { get; set; }
    }
    public interface IDataCrate<Tid, Tmodel, Tdto> : IDataCrate
    {

    }
    internal class DataCrate : IDataCrate
    {
        [Reactive]
        public DataCrateName Name { get; set; }
    }
    internal class DataCrate<Tid, Tmodel, Tdto> : DataCrate, IDataCrate<Tid, Tmodel, Tdto>
    {

    }
}
