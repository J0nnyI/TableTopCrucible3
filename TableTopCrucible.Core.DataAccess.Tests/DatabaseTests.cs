
using AutoMapper;

using DynamicData;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NUnit.Framework;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.DI;
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
        public TestEntity()
        {

        }
        public TestEntity(TestEntityId id, string text)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
        public TestEntity(string text) : this(TestEntityId.From(Guid.NewGuid()), text) { }

        public TestEntityId Id { get; }
        public string Text { get; }
    }
    [AutoMap(typeof(TestEntity), ReverseMap = true)]
    public class TestEntityDTO : IEntityDTO<TestEntityId, TestEntity>
    {
        public Guid IdValue { get; set; }
        public string Text { get; set; }
    }

    [TestFixture]
    public class DatabaseTests
    {
        private IServiceProvider di;
        private IDatabase database;
        private Database internalDatabase;
        private CompositeDisposable disposables;

        [SetUp]
        public void BeforeEach()
        {
            disposables = new CompositeDisposable();
            di = DependencyBuilder.GetTestProvider(srv =>
            {
                srv.RemoveAutoMapper();
                srv.AddAutoMapper(Assembly.GetAssembly(typeof(TestEntity)));
                srv.ReplaceFileSystem<MockFileSystem>();
            });
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
            var entityIn = new TestEntity(text);
            entityIn.Id.Should().NotBeNull();
            entityIn.Id.Value.Should().NotBe(default);
            entityIn.Text.Should().Be(text);

            var entity2 = new TestEntity(entityIn.Id, text);
            entity2.Should().BeEquivalentTo(entityIn);

            var mapper = di.GetRequiredService<IMapper>();
            var dto = mapper.Map<TestEntityDTO>(entityIn);
            var entityOut = mapper.Map<TestEntity>(dto);
            entityOut.Should().BeEquivalentTo(entityIn);

            var lstIn = new SourceCache<TestEntity, TestEntityId>(e=>e.Id);
            lstIn.AddOrUpdate(entityIn);
            var lstDto = mapper.Map<IEnumerable<TestEntityDTO>>(lstIn.Items);
            var lstOut = mapper.Map<IEnumerable<TestEntity>>(lstDto);
            lstIn.Items.Should().BeEquivalentTo(lstOut);
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
            internalDatabase.WorkingDirectory.Exists().Should().BeTrue($"the directory '{internalDatabase.WorkingDirectory.Value}' does not exist");
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
        [Test]
        public void InternalFileSystemGeneration()
        {
            throw new NotImplementedException("test not implemented");
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
            throw new NotImplementedException("the table should hold a reference to its own working directory so that a proper cleanup is ensured");
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
            throw new NotImplementedException("incomplete test, add data to be overridden");
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.InitializeFromFile(path);
            BeforeEach();
            database.InitializeFromFile(path, DatabaseInitializationBehavior.Override);
            throw new NotImplementedException("test if data has been overridden");
        }
        [Test]
        public void InitializationFromFileAfterCrash_override()
        {
            true.Should().BeFalse("incomplete test, add data to restored");
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.InitializeFromFile(path);
            BeforeEach();
            database.InitializeFromFile(path, DatabaseInitializationBehavior.Restore);
            throw new NotImplementedException("test if data has been overridden");
        }
    }
}