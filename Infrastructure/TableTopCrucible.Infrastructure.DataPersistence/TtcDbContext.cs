using System.Reflection;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

//dotnet ef migrations add DEV
//dotnet ef migrations remove
namespace TableTopCrucible.Infrastructure.DataPersistence
{
    public class TtcDbContextFactory : IDesignTimeDbContextFactory<TtcDbContext>
    {
        public TtcDbContext CreateDbContext(string[] args)
        {
            return new(false);
        }
    }

    public interface IDatabaseContext
    {
        public DbSet<Item> Items { get; }
        public DbSet<FileData> Files { get; }
        public DbSet<DirectorySetup> DirectorySetups { get; }
        public int SaveChanges();
        public void Migrate();
    }

    public class TtcDbContext : DbContextWithTriggers, IDatabaseContext
    {
        private readonly LibraryFilePath _path;

        internal TtcDbContext(bool migrate = true, LibraryFilePath path = null)
        {
            _path = path ?? LibraryFilePath.WorkingFile;
            if(migrate)
                Migrate();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<FileData> Files { get; set; }
        public DbSet<DirectorySetup> DirectorySetups { get; set; }

        public void Migrate()
        {
            LibraryFilePath.WorkingFile.GetDirectoryPath().Create();
            Database.Migrate();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_path}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("TableTopCrucible.Infrastructure.Models"));


            base.OnModelCreating(modelBuilder);
        }
    }
}