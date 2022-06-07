using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using MoreLinq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Shared.ItemSync.Tests.Services;

[TestFixture]
public class ZipMappingServiceTests
{
    private IZipMappingService _zipMappingService;
    private IZipMappingRepository _zipMappingRepository;
    private IFileRepository _fileRepository;

    [SetUp]
    public void BeforeEach()
    {
        var host = Prepare.ApplicationEnvironment();
        _zipMappingService = host.Services.GetRequiredService<IZipMappingService>();
        _zipMappingRepository = host.Services.GetRequiredService<IZipMappingRepository>();
        _fileRepository = host.Services.GetRequiredService<IFileRepository>();
    }

    [Test]
    public void test()
    {
        var outDir = (DirectoryPath)@"D:\__TestFileDirectory__\UnzipFilePath";
        var zip = (ZipFilePath)@"D:\__TestFileDirectory__\testZipFile.zip";

        outDir.Delete();
        _fileRepository.Data.Items.Should().BeEmpty();

        var res = _zipMappingService.StartScan(zip, outDir);
        res.fileInfos.Should().NotBeEmpty();
        res.mapping.Should().NotBeEmpty();
        res.fileInfos.Should().AllSatisfy(x=>x.Path.Exists().Should().BeTrue());
        // hash matches
        var testFileInfo = res.fileInfos.First();

        var zipOutHashKey = testFileInfo.HashKey;
        var fileSystemHashKey = FileHashKey.Create(testFileInfo.Path);


        zipOutHashKey.Should().BeEquivalentTo(fileSystemHashKey);

        // no second scan
        var res2 = _zipMappingService.StartScan(zip, outDir);
        res2.mapping.Should().BeEmpty();
        res2.fileInfos.Should().BeEmpty();

        // files are not duplicated, removed files are unzipped
        _zipMappingRepository.RemoveRange(_zipMappingRepository.Data.Items);
        var removedFiles = new[] {
            _fileRepository.Data.Items.ElementAt(0),
            _fileRepository.Data.Items.ElementAt(1)
        };
        _fileRepository.RemoveRange(removedFiles);
        removedFiles.ForEach(x => x.Path.Delete());

        var res3 = _zipMappingService.StartScan(zip, outDir);
        res3.mapping.Should().HaveCount(2);
        res3.fileInfos.Should().HaveCount(2);
        var comp = removedFiles.Select(x => new FileIdentifier(x));
        res3.fileInfos.Select(x => new FileIdentifier(x)).Should().OnlyContain(x=>comp.Contains(x));
        res3.fileInfos.Should().AllSatisfy(x => x.Path.Exists().Should().BeTrue());
    }
    private record FileIdentifier(FilePath filePath, FileHashKey hashKey)
    {
        public FileIdentifier(FileData data) : this(data.Path, data.HashKey)
        {

        }
    }
}
