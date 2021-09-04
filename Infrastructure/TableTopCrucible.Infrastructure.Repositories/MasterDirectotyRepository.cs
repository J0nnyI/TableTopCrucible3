
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
    [Singleton(typeof(FileArchiveRepository))]
    public interface IFileArchiveRepository :
        ISourceRepository<FileArchiveId, FileArchive, FileArchiveDto>
    {
        IObservable<IEnumerable<FileArchivePath>> TakenDirectoriesChanges { get; }
    }
    internal class FileArchiveRepository :
        SourceRepositoryBase<FileArchiveId, FileArchive, FileArchiveDto>,
        IFileArchiveRepository
    {
        public FileArchiveRepository(IDatabase database) : base(database)
        {
            TakenDirectoriesChanges =
                DataChanges
                    .Connect()
                    .Transform(m => m.Path)
                    .ToCollection()
                    .Replay(1);
        }
        public IObservable<IEnumerable<FileArchivePath>> TakenDirectoriesChanges { get; }

    }
}
