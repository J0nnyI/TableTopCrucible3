
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
using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.FileManagement.Exceptions;
using TableTopCrucible.Core.FileManagement.Models;
using TableTopCrucible.Core.FileManagement.ValueTypes;

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
        private Database internalDatabase;
        private CompositeDisposable disposables;

        [SetUp]
        public void BeforeEach()
        {
            disposables = new CompositeDisposable();
            di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
            database = di.GetRequiredService<IDatabase>();
            internalDatabase = database as Database;
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
            //checkiong defaults
            database.Should().NotBeNull();
            internalDatabase.WorkingDirectory.Should().BeNull();
            database.State.Should().Be(DatabaseState.Closed);
            // initializing
            database.Initialize();
            internalDatabase.WorkingDirectory.Should().NotBeNull();
            database.State.Should().Be(DatabaseState.Open);
            internalDatabase.WorkingDirectory.Exists().Should().BeTrue();
            // checking table access
            var table = database.GetTable<TestEntityId, TestEntity, TestEntityDTO>();
            var table2 = database.GetTable<TestEntityId, TestEntity, TestEntityDTO>();
            table.Should().BeSameAs(table2);

            // closing the database
            database.Close();
            database.State.Should().Be(DatabaseState.Closed);
            internalDatabase.WorkingDirectory.Should().BeNull();

            table.State.Should().Be(DatabaseState.Closed);
        }
        //[Test]
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
                .Subscribe(entity => resultEntity = entity)
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
            Action act = () => database.Initialize();
            act.Should().Throw<DatabaseAlreadyOpenedException>();
        }
        [Test]
        public void SaveWithoutLocation()
        {
            database.Initialize();
            Action act = () => database.Save();
            act.Should().Throw<NoDatabaseLocationSelectedException>();
        }
        [Test]
        public void InitializationAfterCrash()
        {
            true.Should().BeFalse("incomplete test, add data to be reloaded");
            database.Initialize();
            BeforeEach();
            Action act = () => database.Initialize();
            // it is expected that it loads the old data
            act.Should().NotThrow();
        }
        [Test]
        public void InitializationFromFileAfterCrash_cancel()
        {
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.InitializeFromFile(path);
            BeforeEach();
            Action cancel = () => database.InitializeFromFile(path, DatabaseInitializationBehavior.Cancel);
            cancel.Should().Throw<OldDatabaseVersionFoundException>();
        }
        [Test]
        public void InitializationFromFileAfterCrash_restore()
        {
            Assert.Fail("incomplete test, add data to be overridden");
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.InitializeFromFile(path);
            BeforeEach();
            database.InitializeFromFile(path, DatabaseInitializationBehavior.Override);
            Assert.Fail("test if data has been overridden");
        }
        [Test]
        public void InitializationFromFileAfterCrash_override()
        {
            true.Should().BeFalse("incomplete test, add data to restored");
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.InitializeFromFile(path);
            BeforeEach();
            database.InitializeFromFile(path, DatabaseInitializationBehavior.Restore);
            Assert.Fail("test if data has been overridden");
        }
    }
}