
using DynamicData;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories
{
    [Singleton]
    public interface IDirectorySetupRepository :
        ISourceRepository<DirectorySetupId, DirectorySetup, DirectorySetupDto>
    {
        IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }
    }
    internal class DirectorySetupRepository :
        SourceRepositoryBase<DirectorySetupId, DirectorySetup, DirectorySetupDto>,
        IDirectorySetupRepository
    {
        public DirectorySetupRepository(IDatabase database) : base(database)
        {
            TakenDirectoriesChanges =
                DataChanges
                    .Connect()
                    .Transform(m => m.Path)
                    .ToCollection()
                    .Replay(1);
        }
        public IObservable<IEnumerable<DirectorySetupPath>> TakenDirectoriesChanges { get; }

    }
}
