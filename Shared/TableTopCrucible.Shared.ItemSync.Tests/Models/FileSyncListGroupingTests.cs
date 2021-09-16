﻿using NUnit.Framework;
using TableTopCrucible.Shared.ItemSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using FluentAssertions;
using Microsoft.Xaml.Behaviors.Core;
using MoreLinq.Extensions;
using Splat;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Shared.ItemSync.Services;


namespace TableTopCrucible.Shared.ItemSync.Models.Tests
{
    [TestFixture()]
    public class FileSyncListGroupingTests
    {
        public static readonly TimeSpan TestTimeout = new TimeSpan(0, 0, 0, 0, 500);

        private IDirectorySetupRepository directorySetupRepository;
        private IScannedFileRepository fileRepository;
        private IFileSynchronizationService fileSyncService;

        internal class FileSetupData
        {
            public FilePath File { get; }
            public string Content { get; init; }
            public string NewContent { get; init; }
            public FileState FileState { get; }
            public FileHashKey HashKey { get; private set; }
            public FileHashKey TargetHashKey { get; private set; }
            public DateTime LastWrite { get; private set; }
            public DateTime TargetLastWrite { get; private set; }
            public ScannedFileDataId OriginalId { get; private set; }


            public FileSetupData(string path, FileState state)
            {
                File = FilePath.From(path);
                Content = Guid.NewGuid().ToString();
                NewContent = Guid.NewGuid().ToString();
            }
            // prepares data for this file according to its given state
            public ScannedFileData Prepare()
            {
                TargetLastWrite = LastWrite = DateTime.Now.AddMinutes(-10);
                OriginalId = ScannedFileDataId.New();

                // create local file
                if (FileState != FileState.Deleted)
                {
                    File.SetCreationTime(LastWrite);
                    File.GetDirectoryPath().Create();
                    File.WriteAllText(Content);
                }

                // new files do not have a model
                if (FileState == FileState.New)
                    return null;

                // create hash for old file
                TargetHashKey = HashKey = FileHashKey.Create(File);

                // update file if required
                if (FileState == FileState.Updated)
                {
                    if (NewContent != null)
                        File.WriteAllText(NewContent);
                    TargetLastWrite = LastWrite.AddMinutes(5);
                    File.SetCreationTime(TargetLastWrite);
                    TargetHashKey = FileHashKey.Create(File);
                }

                // an unchanged file does not need further modifications
                return new ScannedFileData(HashKey, File, LastWrite, OriginalId);

            }

            public Unit Test(ScannedFileData output)
            {
                output.HashKey.Should().Be(HashKey);

                if (FileState == FileState.Deleted)
                    output.Should().BeNull();
                else
                {
                    output.HashKey.Should().Be(TargetHashKey);
                    output.FileLocation.Should().Be(File);
                    output.LastWrite.Should().Be(TargetLastWrite);
                    output.Id.Should().Be(OriginalId);
                }
                
                return Unit.Default;
            }

            
        }


        internal class TestSetup
        {
            private readonly IDirectorySetupRepository directorySetupRepository;
            private readonly IScannedFileRepository fileRepository;
            private readonly IFileSynchronizationService fileSyncService;
            public IEnumerable<FileSetupData> FileData { get; init; }
            public IEnumerable<string> Directories { get; init; }

            public TestSetup()
            {
                this.directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
                this.fileRepository = Locator.Current.GetService<IScannedFileRepository>();
                this.fileSyncService = Locator.Current.GetService<IFileSynchronizationService>();
            }

            private void prepare()
            {
                directorySetupRepository.AddOrUpdate(Directories.Select(dir => new DirectorySetup(dir, dir)));
                fileRepository.AddOrUpdate(FileData.Select(file => file.Prepare()).ToArray());
            }

            private void evaluateResult()
            {
                // files are not duplicated
                fileRepository
                    .Data
                    .Items
                    .Select(file => file.FileLocation)
                    .Distinct()
                    .Count()
                    .Should()
                    .Be(
                        fileRepository
                            .Data
                            .Items
                            .Count());

                // data matches expectations
                FileData.FullJoin(
                    fileRepository.Data.Items,
                    input => input.File,
                    output => output.FileLocation,
                    input => input.Test(null),
                    output =>
                    {
                        Assert.Fail("got a scanned file for which there was no input");
                        return Unit.Default;
                    },
                    (input, output) => input.Test(output)
                ).ToArray();
            }

            public void RunTest()
            {
                prepare();

                fileSyncService
                    .StartScan()
                    .Select(_ => true)
                    .Timeout(TestTimeout, Observable.Return(false))
                    .Wait()
                    .Should()
                    .BeTrue("test run into timeout of " + TestTimeout);

                evaluateResult();
            }
        }

        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();

            this.directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
            this.fileRepository = Locator.Current.GetService<IScannedFileRepository>();
            this.fileSyncService = Locator.Current.GetService<IFileSynchronizationService>();
        }

        [Test]
        public void UpdateFileHashesTest()
        {
            new TestSetup
            {
                Directories = new[]
                {
                    @"C:\first",
                    @"C:\first\Unchanged",
                    @"C:\second",
                },
                FileData = new FileSetupData[]
                {
                    new(@"C:\first\New\A.stl", FileState.New),
                    new(@"C:\first\New\B.stl", FileState.New),
                    new(@"C:\first\Unchanged\A.stl", FileState.Unchanged),
                    new(@"C:\first\Unchanged\B.stl", FileState.Unchanged),
                    new(@"C:\first\Deleted\A.stl", FileState.Deleted),
                    new(@"C:\first\Deleted\B.stl", FileState.Deleted),
                    new(@"C:\first\Updated\A.stl", FileState.Updated),
                    new(@"C:\first\Updated\B.stl", FileState.Updated),
                    new(@"C:\second\A.stl", FileState.New),
                    new(@"C:\second\B.stl", FileState.New),
                },
            }.RunTest();
        }
    }
}