
using AutoMapper;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using ReactiveUI;

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Disposables;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.FileManagement.Exceptions;
using TableTopCrucible.Core.FileManagement.Models;

using ValueOf;

namespace TableTopCrucible.Core.FileManagement.Tests
{
    public class TestEntityId : ValueOf<Guid, TestEntityId>, IEntityId
    {
        public Guid GetGuid()
            => this.Value;
    }
    public class TestEntity : IEntity<TestEntityId>
    {
        public TestEntity(TestEntityId id, string text)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
        public TestEntity(string text) : this(TestEntityId.From(Guid.NewGuid()), text) { }

        public TestEntityId Id { get; }
        public string Text { get; }
    }
    [AutoMap(typeof(TestEntity))]
    public class TestEntityDTO : IEntityDTO<TestEntityId, TestEntity>
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }

    [TestFixture]
    public class DataWarehouseFactoryTests
    {
        private IServiceProvider di;
        private IDatabase database;
        private CompositeDisposable disposables;

        [SetUp]
        public void BeforeEach()
        {
            disposables = new CompositeDisposable();
            di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
            database = di.GetRequiredService<IDatabase>();
        }
        [TearDown]
        public void AfterEach()
        {
            disposables.Dispose();
        }
        [Test]
        public void TestEntity()
        {
            string text = "test";
            var entity = new TestEntity(text);
            entity.Id.Should().NotBeNull();
            entity.Id.Value.Should().NotBe(default);
            entity.Text.Should().Be(text);
        }
        [Test]
        public void CreateNewWarehouseTest()
        {
            database.Should().NotBeNull();
            database.Initialize();
            var table = database.GetTable<TestEntityId, TestEntity, TestEntityDTO>();
            var table2 = database.GetTable<TestEntityId, TestEntity, TestEntityDTO>();
            table.Should().BeSameAs(table2);

            database.Close();
            table.State.Should().Be(TableState.Closed);
        }
        [Test]
        public void InternalFileSystemGeneration()
        {

        }
        [Test]
        public void SaveItem()
        {
            var table = database.GetTable<TestEntityId, TestEntity, TestEntityDTO>();
            var testEntity = new TestEntity("data");
            var updatedEntity = new TestEntity(testEntity.Id, "newData");

            TestEntity resultEntity = null;

            table.WatchValue(testEntity.Id)
                .Subscribe(entity=>resultEntity = entity)
                .DisposeWith(disposables);

            resultEntity.Should().BeNull();
            var timestamp = DateTime.Now;
            table.AddOrUpdate(testEntity);
            resultEntity.Should().BeSameAs(testEntity);
            table.LastChange.Should().BeAfter(timestamp);
            timestamp = DateTime.Now;
            table.AddOrUpdate(updatedEntity);
            resultEntity.Should().BeSameAs(updatedEntity);
            table.LastChange.Should().BeAfter(timestamp);

        }
        [Test]
        public void DoubleOpenedTest()
        {
            database.Initialize();
            Action act = ()=>database.Initialize();
            act.Should().Throw<DatabaseAlreadyOpenedException>();
        }
        [Test]
        public void SaveWithoutLocation()
        {
            database.Initialize();
            Action act = () => database.Save();
            act.Should().Throw<NoDatabaseLocationSelectedException>();
        }
    }
}