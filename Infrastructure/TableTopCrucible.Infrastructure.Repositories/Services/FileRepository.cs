using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services;

[Singleton]
public interface IFileRepository
    : IRepository<FileDataId, FileData>
{
    IEnumerable<FileData> this[DirectoryPath dirPath] { get; }
    IEnumerable<FileData> this[FileHashKey hashKey] { get; }
    IObservable<IChangeSet<FileData, FileDataId>> Watch(FileHashKey hashKey);
    IObservable<IChangeSet<FileData, FileDataId>> Watch(IObservable<FileHashKey> hashKeyChanges);

    FileData ByFilepath(FilePath path);
    IEnumerable<FileData> ByFilepath(IEnumerable<FilePath> filePaths);
    FileData SingleByHashKey(FileHashKey fileKey);
    IObservable<FileData> WatchSingle(IObservable<FileHashKey> hashKeyChanges);
    IObservable<FileData> WatchSingle(FileHashKey hashKey);
}

internal class FileRepository
    : RepositoryBase<FileDataId, FileData>,
        IFileRepository
{
    public FileRepository(IStorageController storageController)
        : base(storageController, storageController.Files)
    {
    }

    /// <summary>
    ///     all files with a given hashKey
    /// </summary>
    /// <param name="hashKey"></param>
    /// <returns></returns>
    public IEnumerable<FileData> this[FileHashKey hashKey]
        => hashKey is null
            ? Enumerable.Empty<FileData>()
            : Data.Items.Where(file => file.HashKey == hashKey);

    /// <summary>
    ///     all files with in a given directory
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    public IEnumerable<FileData> this[DirectoryPath dirPath]
        => dirPath is null
            ? Enumerable.Empty<FileData>()
            : Data.Items.Where(file => dirPath.ContainsFilepath(file.Path));

    public IObservable<IChangeSet<FileData, FileDataId>> Watch(FileHashKey hashKey)
        => Data.Connect().Filter(x => x.HashKey == hashKey);

    public IObservable<FileData> WatchSingle(IObservable<FileHashKey> hashKeyChanges)
        => hashKeyChanges.Select(WatchSingle).Switch();

    public IObservable<FileData> WatchSingle(FileHashKey hashKey)
        => Data.Connect()
            .Filter(x => x.HashKey == hashKey)
            .ToCollection()
            .Select(col =>
                col.FirstOrDefault(file => file.Path.Exists()))
            .DistinctUntilChanged();

    public IObservable<IChangeSet<FileData, FileDataId>> Watch(IObservable<FileHashKey> hashKeyChanges)
        => hashKeyChanges
            .Select(Watch)
            .Switch();

    public FileData ByFilepath(FilePath path)
        => Data.Items.SingleOrDefault(file => file.Path == path);

    public IEnumerable<FileData> ByFilepath(IEnumerable<FilePath> filePaths)
        => filePaths.Select(ByFilepath).Where(file => file != null);

    public FileData SingleByHashKey(FileHashKey fileKey)
        => this[fileKey].FirstOrDefault(file => file.Path.Exists());
}