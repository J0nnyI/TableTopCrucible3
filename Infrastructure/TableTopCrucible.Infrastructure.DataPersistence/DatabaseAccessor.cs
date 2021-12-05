using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    [Singleton]
    public interface IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataEntity> Files { get; }
        public DbSet<DirectorySetupEntity> DirectorySetup { get; }
        void Open(LibraryFilePath file);
        void Save();
    }

    public abstract class DatabaseAccessor : IDatabaseAccessor
    {
        public DbSet<ItemEntity> Items => _database.Items;
        public DbSet<ScannedFileDataEntity> Files => _database.Files;
        public DbSet<DirectorySetupEntity> DirectorySetup => _database.DirectorySetups;

        private IDatabaseContext _database;
        public void Open(LibraryFilePath file)
            => _database = new DatabaseContext(file);

        public void Save()
            => _database.SaveChanges();
    }
}
