using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IFileRepository
        : IRepository<FileDataId, FileData>
    {
        IObservable<IQueryable<FileData>> Watch(FileHashKey hashKey);
        IObservable<IQueryable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges);
    }

    internal class FileRepository
        : RepositoryBase<FileDataId, FileData>,
            IFileRepository
    {
        public FileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {

        }

        public IObservable<IQueryable<FileData>> Watch(FileHashKey hashKey)
        {
            return
                Updates.Where(x =>
                    x.UpdateInfo.ChangeReason == EntityUpdateChangeReason.Init ||
                    x.UpdateInfo
                        .UpdatedEntities
                        .Any(x =>
                            x.Value.HashKey == hashKey)
                )
                .Select(x =>
                    hashKey is null
                        ? Enumerable.Empty<FileData>().AsQueryable()
                        : x.Queryable.Where(file => file.HashKeyRaw == hashKey.Value));
        }
        public IObservable<IQueryable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges)
            => hashKeyChanges
                .Select(Watch)
                .Switch();

        public override string TableName => FileDataConfiguration.ImageTableName;
    }
}