using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;

namespace TableTopCrucible.Infrastructure.DataPersistence.EntityFramework
{
    public interface IDatabaseContext
    {
        public LibraryFilePath FileName { get; }
        public DbSet<ItemEntity> Items { get; }
        public DbSet<ScannedFileDataDto> Files { get; }
        public DbSet<DirectorySetupDto> DirectorySetup { get; }

    }
    internal class DatabaseContext : DbContext, IDatabaseContext
    {
        public LibraryFilePath FileName { get; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ScannedFileDataDto> Files { get; set; }
        public DbSet<DirectorySetupDto> DirectorySetup { get; set; }

        internal DatabaseContext()
        {

        }

        public DatabaseContext(LibraryFilePath fileName)
        {
            FileName = fileName;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={FileName}");


    }
}
