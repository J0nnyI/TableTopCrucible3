using System.Runtime.InteropServices;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    public interface IDatabaseContext
    {
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataEntity> Files { get; }
        public DbSet<DirectorySetupEntity> DirectorySetups { get; }
        public int SaveChanges();
        public void Migrate();
    }

    public class DatabaseContext : DbContextWithTriggers, IDatabaseContext
    {
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ScannedFileDataEntity> Files { get; set; }
        public DbSet<DirectorySetupEntity> DirectorySetups { get; set; }

        public void Migrate()
            => Database.Migrate();

        public DatabaseContext()
        {
            Migrate();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={LibraryFilePath.WorkingFile}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            modelBuilder.Entity<ItemEntity>()
                .HasKey(item => item.Id);
            modelBuilder.Entity<ItemEntity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            modelBuilder.Entity<ScannedFileDataEntity>()
                .HasKey(file => file.Id);
            modelBuilder.Entity<DirectorySetupEntity>()
                .HasKey(directorySetup => directorySetup.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}