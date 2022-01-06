using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Infrastructure.Repositories.Tests.Helper;

namespace TableTopCrucible.Infrastructure.Repositories.Tests.Services
{
    [TestFixture]
    public class MultiRepositoryTests
    {
        private IDatabaseAccessor _databaseAccessor;
        private IDirectorySetupRepository _dirRepo;
        private IItemRepository _itemRepo;
        private IFileRepository _fileRepo;
        private CompositeDisposable _disposables;

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
            _dirRepo = services.GetRequiredService<IDirectorySetupRepository>();
            _fileRepo = services.GetRequiredService<IFileRepository>();
            _itemRepo = services.GetRequiredService<IItemRepository>();
        }

        [TearDown]
        public void AfterEach()
        {
            _dirRepo.Clear();
            _fileRepo.Clear();
            _itemRepo.Clear();
            _disposables.Dispose();
        }


        [TestCase]
        public void File_Item()
        {
            var hash = Enumerable.Range(1, 64).Select(x => (byte)x).ToArray();

            var hashKey = FileHashKey.From((FileSize)100, (FileHash)hash);

            var item = new Item((Name)"item",hashKey,new []{(Tag)"firstTag",(Tag)"secondTag"});

            var file = new FileData((FilePath)@"G:\TestDir", hashKey, DateTime.Now);

            _itemRepo.Add(item);
            _fileRepo.Add(file);
            item.Files.Should().HaveCount(1);
            item.Files[0].Should().Be(file);
            file.Item.Should().Be(item);

        }

    }
}
