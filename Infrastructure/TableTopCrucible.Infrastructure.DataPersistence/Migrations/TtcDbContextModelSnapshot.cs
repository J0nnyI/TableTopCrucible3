﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TableTopCrucible.Infrastructure.DataPersistence;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    [DbContext(typeof(TtcDbContext))]
    partial class TtcDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.DirectorySetup", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.HasKey("Guid");

                    b.ToTable("Directories");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.FileData", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashKeyRaw")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastWrite")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid")
                        .HasName("Id");

                    b.HasIndex("HashKeyRaw")
                        .HasDatabaseName("HashKey");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.ImageData", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashKeyRaw")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid")
                        .HasName("Id");

                    b.HasIndex("HashKeyRaw")
                        .IsUnique();

                    b.ToTable("Images");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.Item", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileKey3dRaw")
                        .HasColumnType("TEXT");

                    b.Property<string>("RawTags")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid")
                        .HasName("Id");

                    b.HasIndex("FileKey3dRaw")
                        .IsUnique();

                    b.ToTable("Items");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.DirectorySetup", b =>
                {
                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.DirectoryPath", "Path", b1 =>
                        {
                            b1.Property<Guid>("DirectorySetupGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Path");

                            b1.HasKey("DirectorySetupGuid");

                            b1.ToTable("Directories");

                            b1.WithOwner()
                                .HasForeignKey("DirectorySetupGuid");
                        });

                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("DirectorySetupGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Name");

                            b1.HasKey("DirectorySetupGuid");

                            b1.ToTable("Directories");

                            b1.WithOwner()
                                .HasForeignKey("DirectorySetupGuid");
                        });

                    b.Navigation("Name");

                    b.Navigation("Path");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.FileData", b =>
                {
                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.FilePath", "Path", b1 =>
                        {
                            b1.Property<Guid>("FileDataGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("FileLocation");

                            b1.HasKey("FileDataGuid");

                            b1.ToTable("Files");

                            b1.WithOwner()
                                .HasForeignKey("FileDataGuid");
                        });

                    b.Navigation("Path");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.ImageData", b =>
                {
                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("ImageDataGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Name");

                            b1.HasKey("ImageDataGuid");

                            b1.ToTable("Images");

                            b1.WithOwner()
                                .HasForeignKey("ImageDataGuid");
                        });

                    b.Navigation("Name");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.Item", b =>
                {
                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("ItemGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Name");

                            b1.HasKey("ItemGuid");

                            b1.ToTable("Items");

                            b1.WithOwner()
                                .HasForeignKey("ItemGuid");
                        });

                    b.OwnsOne("TableTopCrucible.Infrastructure.Models.EntityIds.ItemId", "Id", b1 =>
                        {
                            b1.Property<Guid>("ItemGuid")
                                .HasColumnType("TEXT");

                            b1.Property<Guid>("Guid")
                                .HasColumnType("TEXT");

                            b1.Property<Guid>("Value")
                                .HasColumnType("TEXT")
                                .HasColumnName("Id");

                            b1.HasKey("ItemGuid");

                            b1.ToTable("Items");

                            b1.WithOwner()
                                .HasForeignKey("ItemGuid");
                        });

                    b.Navigation("Id");

                    b.Navigation("Name");
                });
#pragma warning restore 612, 618
        }
    }
}
