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
using TableTopCrucible.Infrastructure.Repositories.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IFileRepository
        : IRepository<FileDataId, FileData>
    {
        IObservable<IQueryable<FileData>> Watch(FileHashKey hashKey);
        IObservable<IQueryable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges);
        public IQueryable<FileData> this[FileHashKey hashKey] { get; }
        public FileData ByFilepath(FilePath path);
        public IQueryable<FileData> ByFilepath(IEnumerable<FilePath> filePaths);
    }

    internal class FileRepository
        : RepositoryBase<FileDataId, FileData>,
            IFileRepository
    {
        public FileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {

        }

        public IQueryable<FileData> this[FileHashKey hashKey]
            =>hashKey is null
                ? Enumerable.Empty<FileData>().AsQueryable()
                : this.Data.Where(file => file.HashKeyRaw == hashKey.Value);

        public IObservable<IQueryable<FileData>> Watch(FileHashKey hashKey)
        {
            return
                Updates.Where(update =>
                    update.UpdateInfo.ChangeReason == EntityUpdateChangeReason.Init ||
                    update.UpdateInfo
                        .UpdatedEntities
                        .Any(x =>
                            x.Value.HashKey == hashKey)
                )
                .Select(update =>
                    hashKey is null
                        ? Enumerable.Empty<FileData>().AsQueryable()
                        : update.Queryable.Where(file => file.HashKeyRaw == hashKey.Value));
        }
        public IObservable<IQueryable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges)
            => hashKeyChanges
                .Select(Watch)
                .Switch();

        public FileData ByFilepath(FilePath path)
            => this.Data.SingleOrDefault(dir => path.Value.ToLower().StartsWith(dir.Path.Value.ToLower()));

        public IQueryable<FileData> ByFilepath(IEnumerable<FilePath> filePaths)
            => this.Data.Where(file =>filePaths.Select(path=>path.Value).Contains(file.PathRaw));

        public override string TableName => FileDataConfiguration.ImageTableName;
    }
}