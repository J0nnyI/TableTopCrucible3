using DynamicData;

using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.FileManagement.ValueTypes;

namespace TableTopCrucible.Core.FileManagement
{
    public interface IDataBranch
    {
        DataBranchName Name { get; set; }
    }
    public interface IDataBranch<Tid, Tmodel, Tdto> : IDataBranch
    {

    }
    internal class DataBranch : IDataBranch
    {
        [Reactive]
        public DataBranchName Name { get; set; }
    }
    internal class DataBranch<Tid, Tmodel, Tdto> : DataBranch, IDataBranch<Tid, Tmodel, Tdto>
    {
        private readonly SourceCache<IDataCrate<Tid, Tmodel, Tdto>, DataCrateName> _crates
            = new SourceCache<IDataCrate<Tid, Tmodel, Tdto>, DataCrateName>(crate => crate.Name);
    }
}
