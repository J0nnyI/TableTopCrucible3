using NUnit.Framework;
using TableTopCrucible.Infrastructure.Repositories.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Splat;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services.Tests
{
    [TestFixture()]
    public class DirectorySetupRepositoryTests
    {
        private DirectorySetupRepository _repository;
        private CompositeDisposable _disposables;
        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            _repository = Locator.Current.GetService<DirectorySetupRepository>();
            _disposables = new();
        }

        [TearDown]
        public void AfterEach()
        {
            _disposables.Dispose();
        }

        [TestCase]
        public void SetupWorks()
        {
            _repository.Should().NotBeNull();
            _disposables.Should().NotBeNull();
        }

        [TestCase]
        public void Add()
        {
            var bufferClose = new Subject<Unit>().DisposeWith(_disposables);
            IList<CollectionUpdate<DirectorySetupId, DirectorySetup>> bufferContent = null;
            _repository
                .Updates
                .Buffer(bufferClose)
                .Subscribe(buffer =>
                    bufferContent = buffer
                )
                .DisposeWith(_disposables);

            var newEntity = new DirectorySetup((DirectoryPath)@"C:\testDirRoot\testDir");

            _repository.Add(newEntity);
            bufferClose.OnNext(Unit.Default);

            bufferContent.Should().HaveCount(1);
            var updatedItems = bufferContent.First();

            updatedItems.Should().HaveCount(1);

            var newItem = updatedItems.First();

            newItem.Name.Should().Be(newEntity.Name);
            newItem.Path.Should().Be(newEntity.Path);
            newItem.Id.Should().Be(newEntity.Id);

        }
    }
}