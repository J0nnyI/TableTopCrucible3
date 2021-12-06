using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.ValueTypes;
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
        public void Migrate();
    }
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public LibraryFilePath FileName { get; init; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ScannedFileDataEntity> Files { get; set; }
        public DbSet<DirectorySetupEntity> DirectorySetups { get; set; }
        public void Migrate()
            => this.Database.Migrate();

        public DatabaseContext()
        {
                
        }

        public DatabaseContext(LibraryFilePath fileName)
        {
            FileName = fileName;
            this.Migrate();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={FileName}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemEntity>()
                .HasKey(item => item.Id);
            modelBuilder.Entity<ScannedFileDataEntity>()
                .HasKey(file => file.Id);
            modelBuilder.Entity<DirectorySetupEntity>()
                .HasKey(directorySetup => directorySetup.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
