using DynamicData;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.FileManagement.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement
{
    [Singleton(typeof(DataWarehouseFactory))]
    public interface IDataWarehouseFactory
    {
        public IDataWarehouse OpenWarehouse(FilePath file);
        public IDataWarehouse CreateNewWarehouse();
    }
    internal class DataWarehouseFactory : IDataWarehouseFactory
    {
        public IDataWarehouse CreateNewWarehouse()
        {
            throw new NotImplementedException();
        }

        public IDataWarehouse OpenWarehouse(FilePath file)
        {
            throw new NotImplementedException();
        }
    }



    public interface IDataWarehouse
    {
        void Save();
        void SaveAs(FilePath file);
        void Close();
    }
    internal class DataWarehouse : IDataWarehouse
    {
        private readonly SourceCache<IDataBranch, DataBranchName> _branches
            = new SourceCache<IDataBranch, DataBranchName>(branch => branch.Name);

        internal FilePath file = null;
        internal WorkingDirectoryPath rootDirectory;

        public void Close()
        {
            throw new NotImplementedException();
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
