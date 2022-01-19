using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

using DynamicData;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Reactive.Testing;

using NUnit.Framework;

using Splat;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Exceptions;
using TableTopCrucible.Infrastructure.Repositories.Helper;
using TableTopCrucible.Infrastructure.Repositories.Models;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Infrastructure.Repositories.Tests.Helper;

namespace TableTopCrucible.Infrastructure.Repositories.Tests.Services
{
    [TestFixture()]
    public class DirectorySetupRepositoryTests
    {
        private IDatabaseAccessor _databaseAccessor;
        private IDirectorySetupRepository _repository;
        private CompositeDisposable _disposables;
        private DirectorySetup _entity;
        private DirectorySetup _falseEntity;

        public IEnumerable<DirectorySetup> getMultiple(int count = 10)
            => Enumerable.Range(1, count).Select(
                x => new DirectorySetup((DirectoryPath)@$"G:\Folder {x}"));

        [SetUp]
        public void BeforeEach()
        {
            _disposables = new();

            //var host = Prepare.ApplicationEnvironment().DisposeWith(_disposables);
            //// it is important not to use locator.current since it is unable to provide the services when the tests start.
            //var services = host.Services;

            var services = Prepare.Services().BuildServiceProvider().DisposeWith(_disposables);
            _databaseAccessor = services.GetRequiredService<IDatabaseAccessor>();
            _repository = services.GetRequiredService<IDirectorySetupRepository>();

            _entity = new DirectorySetup((DirectoryPath)@"G:\Folder_OK");
            _falseEntity = new DirectorySetup((DirectoryPath)@"G:\Folder_NotOk");
        }

        [TearDown]
        public void AfterEach()
        {
            _repository.Clear();
            _disposables.Dispose();
        }

        [TestCase]
        public void SetupWorks()
        {
            _databaseAccessor.Should().NotBeNull();
            _repository.Should().NotBeNull();
            _disposables.Should().NotBeNull();
            _repository.Data.Get("",x=>x).Should().BeEmpty();
        }

        [TestCase]
        public void AddSingleItem()
        {
            _repository.Add(_entity);
            _repository.Add(_falseEntity);
            _repository.Data.Get("", x => x).Should().HaveCount(2);
            new Action(() => _repository.Add(_entity)).Should().ThrowExactly<EntityAlreadyAddedException<DirectorySetupId, DirectorySetup>>();
        }
        [TestCase]
        [Ignore("todo")]
        public void AddMultipleItems()
        {
        }

        [TestCase]
        public void Clear()
        {
            var count = 5;
            _repository.AddRange(getMultiple(5));
            _repository.Data.Get("", x => x).Should().HaveCount(count);
            _repository.Clear();
            _repository.Data.Get("", x => x).Should().BeEmpty();
        }

        [TestCase]
        public void Indexer()
        {
            _repository.Add(_entity);
            _repository.Add(_falseEntity);

            var entityById = _repository[_entity.Id];
            var entityByPath = _repository[_entity.Path];

            entityById.Should()
                .Be(entityByPath)
                .And.Be(_entity);
        }

        [TestCase]
        public void Watch_SingleEdit()
        {
            var bufferClose = new Subject<Unit>();
            IList<DirectorySetup> bufferContent = null;

            _repository.Add(_falseEntity);
            _repository.Watch(_entity.Id)
                .Buffer(bufferClose)
                .Subscribe(x => bufferContent = x)
                .DisposeWith(_disposables);

            bufferClose.OnNext();
            bufferContent.Count.Should().Be(1);
            bufferContent[0].Should().BeNull();

            _repository.Add(_entity);
            bufferClose.OnNext();
            bufferContent.Should().HaveCount(1);
            bufferContent[0].Should().Be(_entity);

            _repository.Remove(_entity);
            bufferClose.OnNext();
            bufferContent.Should().HaveCount(1);
            bufferContent[0].Should().BeNull();
        }

        [TestCase]
        public void Watch_MultiEdit()
        {
            var bufferClose = new Subject<Unit>();
            IList<DirectorySetup> bufferContent = null;

            _repository.AddRange(_falseEntity.AsArray());
            _repository.Watch(_entity.Id)
                .Buffer(bufferClose)
                .Subscribe(x => bufferContent = x)
                .DisposeWith(_disposables);

            bufferClose.OnNext();
            bufferContent.Count.Should().Be(1);
            bufferContent[0].Should().BeNull();

            _repository.AddRange(_entity.AsArray());
            bufferClose.OnNext();
            bufferContent.Should().HaveCount(1);
            bufferContent[0].Should().Be(_entity);

            _repository.RemoveRange(_entity.AsArray());
            bufferClose.OnNext();
            bufferContent.Should().HaveCount(1);
            bufferContent[0].Should().BeNull();
        }

        [TestCase]
        public void ToCache_Single_OnlyItem()
        {
            var cache = _repository.Updates.ToObservableCache(_disposables)
                .AsObservableCache();

            cache.Items.Should().BeEmpty();

            _repository.Add(_entity);

            var res = cache.Lookup(_entity.Id);
            var res2 = _repository[_entity.Id];
            res.HasValue.Should().BeTrue();
            res.Value.Should()
                .BeSameAs(res2)
                .And.NotBeNull()
                .And.BeSameAs(_entity);
            cache.Items.Should().HaveCount(1);

            _repository.Remove(_entity);

            cache.Lookup(_entity.Id).HasValue.Should().BeFalse();
        }

        [TestCase]
        [Ignore("todo")]
        public void Updates()
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

            updatedItems.DbSet.Get("", x => x).Should().HaveCount(1);

            var newItem = updatedItems.DbSet.Get("", x => x).First();

            newItem.Name.Should().Be(newEntity.Name);
            newItem.Path.Should().Be(newEntity.Path);
            newItem.Id.Should().Be(newEntity.Id);

        }
    }
}