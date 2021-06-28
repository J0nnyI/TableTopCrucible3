using DynamicData;

using Splat;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.FileManagement
{
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
        void InitializeFromFile(FilePath file);
        void Initialize();


    }
    internal class Database : IDatabase
    {
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
            throw new NotImplementedException();
        }

        public void InitializeFromFile(FilePath file)
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
