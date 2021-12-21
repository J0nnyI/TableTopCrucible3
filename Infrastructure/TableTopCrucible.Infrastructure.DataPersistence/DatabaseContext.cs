using System.Reflection;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    public interface IDatabaseContext
    {
        public DbSet<Item> Items { get; }
        public DbSet<FileData> Files { get; }
        public DbSet<DirectorySetup> DirectorySetups { get; }
        public int SaveChanges();
        public void Migrate();
    }

    public class DatabaseContext : DbContextWithTriggers, IDatabaseContext
    {
        public DatabaseContext()
        {
            //Migrate();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<FileData> Files { get; set; }
        public DbSet<DirectorySetup> DirectorySetups { get; set; }

        public void Migrate()
            => Database.Migrate();


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={LibraryFilePath.WorkingFile}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("TableTopCrucible.Infrastructure.Models"));
            
            
            base.OnModelCreating(modelBuilder);
        }
    }
}