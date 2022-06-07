using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.ItemSync.Services;
[Singleton]
public interface IZipMappingService
{
    //(IEnumerable<ZipMapping> mapping, IEnumerable<FileData> fileInfos) StartScan(ZipFilePath filePath, DirectoryPath fileRootDirectory);
}
public class ZipMappingService : IZipMappingService
{
    private readonly IZipMappingRepository _zipMappingRepository;
    private readonly IFileRepository _fileRepository;

    public ZipMappingService(IZipMappingRepository zipMappingRepository, IFileRepository fileRepository)
    {
        _zipMappingRepository = zipMappingRepository;
        _fileRepository = fileRepository;
    }

    private IObservable<(IEnumerable<ZipMapping> mapping, IEnumerable<FileData> fileInfos)> ScanMany(IEnumerable<FileData> files, ICompositeTracker tracker)
    {
        Observable.Start(() =>
        {
            var hashAlgorithm = SHA512.Create();
            files.Where(file => file.Path.GetExtension().IsArchive()).Select(file => new
            {
                file,
                tracker = tracker.AddSingle((Name)file.Path.Value, (JobWeight)file.Path.GetSize().Value)
            }).ToList()
            .ForEach(x =>
            {
                //ScanSingle(x.file.Path, fileRootDirectory,)
            });


        },RxApp.TaskpoolScheduler);
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileRootDirectory"></param>
    /// <returns>the mapping and fileInfo for the extracted files. empty if it was already registered and unzipped</returns>
    public (IEnumerable<ZipMapping> mapping, IEnumerable<FileData> fileInfos)
        ScanSingle(ZipFilePath filePath, DirectoryPath fileRootDirectory, ISourceTracker tracker, FileHashKey hashKey, SHA512 hashAlgorithm)
    {
        try
        {
            using var stream = filePath.OpenRead();
            //using var hashAlgorithm = SHA512.Create();

            //var hashKey = FileHashKey.Create(filePath.GetSize(), stream, hashAlgorithm);

            if (_zipMappingRepository.Data.Items.Any(x => x.ZipFileHash == hashKey))
                return new(Enumerable.Empty<ZipMapping>(), Enumerable.Empty<FileData>());

            var res = startScan(stream, fileRootDirectory, hashKey, (Description)filePath.Value, hashAlgorithm);
            _fileRepository.AddRange(res.fileInfos);
            _zipMappingRepository.AddRange(res.mapping);
            return res;
        }
        finally
        {
            tracker.Complete();
        }
    }

    private (
            IEnumerable<ZipMapping> mapping,
            IEnumerable<FileData> fileInfos
        ) startScan(
            Stream stream,
            DirectoryPath fileRootDirectory,
            FileHashKey zipHashKey,
            Description source,
            SHA512 hashAlgorithm = null
        )
    {
        List<ZipMapping> nestedZips = new();
        List<ZipContent> content = new();
        List<FileData> fileInfos = new();
        hashAlgorithm ??= SHA512.Create();


        using var archive = new ZipArchive(stream);

        fileRootDirectory.Create();

        foreach (var entry in archive.Entries)
        {

            // directory
            if (entry.Name == string.Empty)
                continue;
            using var compressionStream = entry.Open();

            var entryhashKey = FileHashKey.Create((FileSize)entry.Length, entry.Open(), hashAlgorithm);

            var relativeFilePath = (RelativeFilePath)entry.FullName;
            var fileType = relativeFilePath.GetExtension().GetFileType();
            content.Add(new(fileType, entryhashKey, relativeFilePath));

            if (fileType == FileType.Archive)
            {
                var nestedPath = (DirectoryPath)Path.Combine(
                    fileRootDirectory.Value,
                    Path.GetDirectoryName(entry.FullName)!,
                    Path.GetFileNameWithoutExtension(entry.Name));

                var nestedResult = startScan(entry.Open(), nestedPath, entryhashKey, source + " / " + entry.FullName);
                nestedZips.AddRange(nestedResult.mapping);
                fileInfos.AddRange(nestedResult.fileInfos);
                continue;
            }

            if (_fileRepository.Data.Items.None(x => x.HashKey == entryhashKey))
            {
                var filePath = fileRootDirectory + relativeFilePath;
                if (filePath.Exists())
                    filePath = filePath.AddGuid();
                filePath.Write(entry.Open());
                var fileInfo = new FileData(filePath, entryhashKey, filePath.GetLastWriteTime());
                fileInfos.Add(fileInfo);
            }
        }
        nestedZips.Add(new ZipMapping(zipHashKey, content) { Source = source });
        return new(nestedZips, fileInfos);
    }
}
