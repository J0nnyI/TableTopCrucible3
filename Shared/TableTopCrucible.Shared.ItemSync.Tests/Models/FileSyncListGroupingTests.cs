﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;
using Splat;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Models;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Shared.ItemSync.Tests.Models
{
    [TestFixture]
    public class FileSyncListGroupingTests
    {
        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();

            directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
            fileRepository = Locator.Current.GetService<IFileRepository>();
            fileSyncService = Locator.Current.GetService<IFileSynchronizationService>();
        }

        public static readonly TimeSpan TestTimeout = TimeSpan.FromMilliseconds(500);

        private IDirectorySetupRepository
            directorySetupRepository;

        private IFileRepository fileRepository;
        private IFileSynchronizationService fileSyncService;

        internal class FileSetupData
        {
            public FileSetupData(string path, FileUpdateSource updateSource)
            {
                FileUpdateSource = updateSource;
                File = FilePath.From(path);
                Content = Guid.NewGuid().ToString();
                NewContent = Guid.NewGuid().ToString();
            }

            public FilePath File { get; }
            public string Content { get; init; }
            public string NewContent { get; init; }
            public FileUpdateSource FileUpdateSource { get; }
            public FileHashKey HashKey { get; private set; }
            public FileHashKey TargetHashKey { get; private set; }
            public DateTime LastWrite { get; private set; }
            public DateTime TargetLastWrite { get; private set; }
            public FileDataId OriginalId { get; private set; }

            // prepares data for this file according to its given state
            public FileData Prepare()
            {
                TargetLastWrite = LastWrite = DateTime.Now.AddMinutes(-10);
                OriginalId = FileDataId.New();

                // create local file
                if (FileUpdateSource != FileUpdateSource.Deleted)
                {
                    File.GetDirectoryPath().Create();
                    File.WriteAllText(Content);
                    File.SetCreationTime(LastWrite);
                }

                // new files do not have a model
                if (FileUpdateSource == FileUpdateSource.New)
                    return null;

                // create hash for old file
                TargetHashKey = HashKey = FileHashKey.Create(File);

                // update file if required
                if (FileUpdateSource == FileUpdateSource.Updated)
                {
                    if (NewContent != null)
                        File.WriteAllText(NewContent);
                    TargetLastWrite = LastWrite.AddMinutes(5);
                    File.SetCreationTime(TargetLastWrite);
                    TargetHashKey = FileHashKey.Create(File);
                }

                throw new NotImplementedException("has to be redone");
                // an unchanged file does not need further modifications
                //return new (HashKey, File, LastWrite, OriginalId);
            }

            public Unit Test(FileData output)
            {
                output.HashKey.Should().Be(HashKey);

                if (FileUpdateSource == FileUpdateSource.Deleted)
                {
                    output.Should().BeNull();
                }
                else
                {
                    output.HashKey.Should().Be(TargetHashKey);
                    output.Path.Should().Be(File);
                    output.LastWrite.Should().Be(TargetLastWrite);
                    output.Id.Should().Be(OriginalId);
                }

                return Unit.Default;
            }
        }


        internal class TestSetup
        {
            private readonly IDirectorySetupRepository directorySetupRepository;
            private readonly IFileRepository fileRepository;
            private readonly IFileSynchronizationService fileSyncService;

            public TestSetup()
            {
                directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
                fileRepository = Locator.Current.GetService<IFileRepository>();
                fileSyncService = Locator.Current.GetService<IFileSynchronizationService>();
            }

            public IEnumerable<FileSetupData> FileData { get; init; }
            public IEnumerable<string> Directories { get; init; }

            private void prepare()
            {
                throw new NotImplementedException("has to be rewritten");
                //directorySetupRepository.Update(
                //    Directories.Select(dir =>
                //        new DirectorySetupChangeSet(
                //            (Name)dir,
                //            DirectorySetupEntity.From(dir)
                //            )
                //        )
                //    );
                //fileRepository.Update(FileData.Select(file => file.Prepare()).Where(file => file != null).ToArray());
            }

            private void evaluateResult()
            {
                throw new NotImplementedException("has to be rewritten");
                // files are not duplicated

                //fileRepository
                //    .Values
                //    .Select(file => file.FileLocation)
                //    .Distinct()
                //    .Count()
                //    .Should()
                //    .Be(
                //        fileRepository
                //            .Values
                //            .Count());

                //// data matches expectations
                //FileData.FullJoin(
                //    fileRepository.Values,
                //    input => input.File,
                //    output => output.FileLocation,
                //    input => input.Test(null),
                //    output =>
                //    {
                //        Assert.Fail("got a scanned file for which there was no input");
                //        return Unit.Default;
                //    },
                //    (input, output) => input.Test(output)
                //).ToArray();
            }

            public void RunTest()
            {
                prepare();

                fileSyncService
                    .StartScan()
                    .OnDone()
                    .Take(1)
                    .Timeout(TestTimeout)
                    .Wait();

                evaluateResult();
            }
        }

        [Test]
        public void UpdateFileHashesTest()
        {
            // feature not implemented yet
            new TestSetup
            {
                Directories = new[]
                {
                    @"C:\First",
                    @"C:\First\Unchanged",
                    @"C:\second"
                },
                FileData = new FileSetupData[]
                {
                    new(@"C:\First\New\A.stl", FileUpdateSource.New),
                    new(@"C:\First\New\B.stl", FileUpdateSource.New),
                    new(@"C:\First\Unchanged\A.stl", FileUpdateSource.Unchanged),
                    new(@"C:\First\Unchanged\B.stl", FileUpdateSource.Unchanged),
                    new(@"C:\First\Deleted\A.stl", FileUpdateSource.Deleted),
                    new(@"C:\First\Deleted\B.stl", FileUpdateSource.Deleted),
                    new(@"C:\First\Updated\A.stl", FileUpdateSource.Updated),
                    new(@"C:\First\Updated\B.stl", FileUpdateSource.Updated),
                    new(@"C:\second\A.stl", FileUpdateSource.New),
                    new(@"C:\second\B.stl", FileUpdateSource.New)
                }
            }.RunTest();
        }
    }
}