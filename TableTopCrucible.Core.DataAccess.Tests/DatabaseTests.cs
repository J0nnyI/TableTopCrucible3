using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;

using AutoMapper;

using DynamicData;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Core.DataAccess.ValueTypes;
using TableTopCrucible.Core.DI;
using TableTopCrucible.Core.ValueTypes;

using ValueOf;

namespace TableTopCrucible.Core.DataAccess.Tests
{
    public class TestEntityId : EntityIdBase<TestEntityId> { }
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
    [AutoMap(typeof(TestEntity), ReverseMap = true)]
    public class TestEntityDto : IEntityDto<TestEntityId, TestEntity>
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
            try
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
            catch (Exception ex)
            {
                Debugger.Break();
                throw new Exception("Setup fails: ", ex);
            }
        }
        [TearDown]
        public void AfterEach()
        {
            database.Close();
            disposables.Dispose();
        }








        public IEnumerable<TestEntity> GenerateTestData(int start = 0, int count = 10)
            => Enumerable.Range(start, count).Select(c => new TestEntity(TestEntityId.New(), $"TestEntity_SingleEntity {c}"));


        public IEnumerable<TestEntity> AddTestData(int start = 0, int count = 10, IDatabase otherDatabase = null)
        {
            var table = GetTestTable(otherDatabase);
            var addedData = GenerateTestData(start, count).ToArray();
            table.AddOrUpdate(addedData);
            return addedData;
        }

        public ITable<TestEntityId, TestEntity, TestEntityDto> GetTestTable(IDatabase otherDatabase = null)
            => (otherDatabase ?? database).GetTable<TestEntityId, TestEntity, TestEntityDto>();














        [Test]
        public void GetTable_SameInstance()
        {
            GetTestTable(database).Should().BeSameAs(GetTestTable(database));
        }

        [Test]
        public void GetTableWhenStillClosed()
            => new Action(() => GetTestTable()).Should().Throw<DatabaseClosedException>();

        [Test]
        public void TestEntity_Single()
        {
            const string text = "test";
            var entityIn = new TestEntity(text);
            entityIn.Id.Should().NotBeNull();
            entityIn.Id.Value.Should().NotBe(default);
            entityIn.Text.Should().Be(text);

            var entity2 = new TestEntity(entityIn.Id, text);
            entity2.Should().BeEquivalentTo(entityIn);

            var mapper = di.GetRequiredService<IMapper>();
            var dto = mapper.Map<TestEntityDto>(entityIn);
            var entityOut = mapper.Map<TestEntity>(dto);
            entityOut.Should().BeEquivalentTo(entityIn);

        }

        [Test]
        public void TestEntity_List()
        {
            var mapper = di.GetRequiredService<IMapper>();

            var lstIn = new SourceCache<TestEntity, TestEntityId>(e => e.Id);
            lstIn.AddOrUpdate(GenerateTestData());
            var lstDto = mapper.Map<IEnumerable<TestEntityDto>>(lstIn.Items);
            var lstOut = mapper.Map<IEnumerable<TestEntity>>(lstDto);
            lstIn.Items.Should().BeEquivalentTo(lstOut);
        }
        [Test]
        public void CreateNewWarehouseTest()
        {
            //checkiong defaults
            database.Should().NotBeNull();
            internalDatabase.LibraryPath.Should().BeNull();
            database.State.Should().Be(DatabaseState.Closed);
            // initializing
            database.New();
            internalDatabase.LibraryPath.Should().NotBeNull();
            database.State.Should().Be(DatabaseState.Open);
            internalDatabase.LibraryPath.Exists().Should().BeTrue($"the directory '{internalDatabase.LibraryPath.Value}' does not exist");
            // checking table access
            var table = database.GetTable<TestEntityId, TestEntity, TestEntityDto>();
            var table2 = database.GetTable<TestEntityId, TestEntity, TestEntityDto>();
            table.Should().BeSameAs(table2);

            // closing the database
            database.Close();
            database.State.Should().Be(DatabaseState.Closed);
            internalDatabase.LibraryPath.Should().BeNull();

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
            var table = database.GetTable<TestEntityId, TestEntity, TestEntityDto>();
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
            throw new NotImplementedException("todo the table should hold a reference to its own working directory so that a proper cleanup is ensured");
            throw new NotImplementedException("todo when closing a table / database, check if its dirty and implement an autosave");
        }
        [Test]
        public void DoubleOpenedTest()
        {
            database.New();
            new Action(() => database.New())
                .Should()
                .Throw<DatabaseAlreadyOpenedException>();
        }
        [Test]
        public void SaveWithoutLocation()
        {
            database.New();
            Action act = () => database.Save();
            act.Should().Throw<NoDatabaseLocationSelectedException>();
        }
        [Test]
        [Ignore("influences other tests")]
        public void InitializationAfterCrash()
        {
            database.New();
            BeforeEach();
            AfterEach();
            Action act = () => database.New();
            //it is expected that it loads the old data
            act.Should().Throw<Exception>();
            throw new NotImplementedException("incomplete test, add data to be reloaded");
        }

        [Test]
        public void SaveAs()
        {
            // setup
            var newFile = LibraryFilePath.From(@"C:\TTC\Tests\temp\test.ttcl");
            var newDir = newFile.GetWorkingDirectory();
            // assert: initial state
            database.State.Should().Be(DatabaseState.Closed, "database not closed");
            // init
            database.New();
            // assert: post init state
            database.State.Should().Be(DatabaseState.Open, "database not open after init");
            ((Database)database).CurrentFile.Should().BeNull("a file has been set despite not having created one");
            var oldDir = ((Database)database).LibraryPath;
            oldDir.Should().NotBeNull("temp working directory has not been set");
            oldDir.Exists().Should().BeTrue("temp working directory has not been created");
            newDir.Exists().Should().BeFalse("the new directory exists before renaming the file");

            var batchSize = 10;
            var addedData = AddTestData(100, batchSize).ToArray();
            var testTable = GetTestTable();
            testTable.Data.Items.Should().BeEquivalentTo(addedData);

            database.SaveAs(newFile);

            oldDir.Exists().Should().BeFalse("the old path still exists after save-as");
            newDir.Exists().Should().BeTrue("the new dir has not been created");
            ((Database)database).LibraryPath.Should().Be(newDir, "the internal library path has not been updated");
            ((Database)database).tables
                .Select(kv=>kv.Value)
                .Cast<Table<TestEntityId,TestEntity,TestEntityDto>>()
                .Select(t=>t.LibraryDirectory)
                .Should()
                .AllBeEquivalentTo(newDir, "items have not been added properly");

            database.Close();

            newDir.Exists().Should().BeFalse("the working directory has not been deleted");

            BeforeEach();
            database.OpenFile(newFile);
            testTable.Data.Items.Should().BeEquivalentTo(addedData, "items have not been loaded properly");



            database.Save();


            
        }

        [Test]
        public void InitializationFromFileAfterCrash_cancel()
        {
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.New();
            database.SaveAs(path);
            BeforeEach();
            Action cancel = () => database.OpenFile(path, DatabaseInitErrorBehavior.Cancel);
            cancel.Should().Throw<OldDatabaseVersionFoundException>();
        }
        [Test]
        public void InitializationFromFileAfterCrash_restore()
        {
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.OpenFile(path);
            BeforeEach();
            database.OpenFile(path, DatabaseInitErrorBehavior.Override);
            throw new NotImplementedException("test if data has been overridden");
        }
        [Test]
        public void InitializationFromFileAfterCrash_override()
        {
            true.Should().BeFalse("incomplete test, add data to restored");
            var path = LibraryFilePath.From(@".\test.ttcl");
            database.OpenFile(path);
            BeforeEach();
            database.OpenFile(path, DatabaseInitErrorBehavior.Restore);
            throw new NotImplementedException("test if data has been overridden");
        }
    }
}