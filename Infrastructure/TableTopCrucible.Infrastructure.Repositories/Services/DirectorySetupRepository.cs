
using DynamicData;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories
{
    [Singleton]
    public interface IDirectorySetupRepository 
    {
        IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }
    }
    internal class DirectorySetupRepository : IDirectorySetupRepository
    {
        private readonly IDatabaseAccessor _database;

        public DirectorySetupRepository(IDatabaseAccessor database)
        {
            _database = database;
            //TakenDirectoriesChanges =
            //    Data
            //        .Connect()
            //        .Transform(m => m.Path)
            //        .ToCollection()
            //        .Replay(1);
        }
        public IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }

        public void Add(IDirectorySetupRepository newModel)
        {
            _database.DirectorySetup.
        }
    }
}
