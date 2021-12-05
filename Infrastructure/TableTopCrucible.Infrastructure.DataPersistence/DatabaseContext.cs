using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    public interface IDatabaseContext
    {
        public LibraryFilePath FileName { get; }
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataEntity> Files { get; }
        public DbSet<DirectorySetupEntity> DirectorySetups { get; }
        public int SaveChanges();
    }
    internal class DatabaseContext : DbContext, IDatabaseContext
    {
        public LibraryFilePath FileName { get; init; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ScannedFileDataEntity> Files { get; set; }
        public DbSet<DirectorySetupEntity> DirectorySetups { get; set; }
        

        public DatabaseContext(LibraryFilePath fileName)
        {
            FileName = fileName;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={FileName}");


    }
}
