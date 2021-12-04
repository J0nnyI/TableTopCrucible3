using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence.EntityFramework;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    [Singleton]
    public interface IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataDto> Files { get; }
        public DbSet<DirectorySetupDto> DirectorySetup { get; }
    }

    public abstract class DatabaseAccessor : IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataDto> Files { get; }
        public DbSet<DirectorySetupDto> DirectorySetup { get; }

        private IDatabaseContext _database;
        public void Open(LibraryFilePath file)
        {
            _database = new DatabaseContext(file);
        }
    }
}
