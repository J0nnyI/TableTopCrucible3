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
        IObservable<IEnumerable<FileData>> Watch(FileHashKey hashKey);
        IObservable<IEnumerable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges);
        public IEnumerable<FileData> this[FileHashKey hashKey] { get; }
        public FileData ByFilepath(FilePath path);
        public IEnumerable<FileData> ByFilepath(IEnumerable<FilePath> filePaths);
    }

    internal class FileRepository
        : RepositoryBase<FileDataId, FileData>,
            IFileRepository
    {
        public FileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {

        }

        public IEnumerable<FileData> this[FileHashKey hashKey]
            => hashKey is null
                ? Enumerable.Empty<FileData>()
                : this.Data.Get($"file by HashKey {hashKey}", data => data.Where(file => file.HashKeyRaw == hashKey.Value));

        public IObservable<IEnumerable<FileData>> Watch(FileHashKey hashKey)
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
                        ? Enumerable.Empty<FileData>()
                        : update.DbSet.Get($"watch by hashKey {hashKey}",data=>data.Where(file => file.HashKeyRaw == hashKey.Value)));
        }
        public IObservable<IEnumerable<FileData>> Watch(IObservable<FileHashKey> hashKeyChanges)
            => hashKeyChanges
                .Select(Watch)
                .Switch();

        public FileData ByFilepath(FilePath path)
            => this.Data.GetSingle($"file by filePath {path}", data => data.SingleOrDefault(dir => path.Value.ToLower().StartsWith(dir.Path.Value.ToLower())));

        public IEnumerable<FileData> ByFilepath(IEnumerable<FilePath> filePaths)
            => this.Data.Get($"file by filePaths {filePaths}", data => data.Where(file => filePaths.Select(path => path.Value).Contains(file.PathRaw)));

        public override string TableName => FileDataConfiguration.ImageTableName;
    }
}