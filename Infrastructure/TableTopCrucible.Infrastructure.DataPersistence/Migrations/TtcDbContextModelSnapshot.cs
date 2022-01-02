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

                    b.ToTable("DirectorySetups");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.FileData", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashKey_Raw")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastWrite")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid")
                        .HasName("Id");

                    b.HasIndex("HashKey_Raw");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.Item", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileKey3d_Raw")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.HasIndex("FileKey3d_Raw");

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

                            b1.ToTable("DirectorySetups");

                            b1.WithOwner()
                                .HasForeignKey("DirectorySetupGuid");
                        });

                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("DirectorySetupGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .HasColumnType("TEXT")
                                .HasColumnName("Name");

                            b1.HasKey("DirectorySetupGuid");

                            b1.ToTable("DirectorySetups");

                            b1.WithOwner()
                                .HasForeignKey("DirectorySetupGuid");
                        });

                    b.Navigation("Name");

                    b.Navigation("Path");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.FileData", b =>
                {
                    b.HasOne("TableTopCrucible.Infrastructure.Models.Entities.Item", "Item")
                        .WithMany("Files")
                        .HasForeignKey("HashKey_Raw")
                        .HasPrincipalKey("FileKey3d_Raw");

                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.FilePath", "FileLocation", b1 =>
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

                    b.Navigation("FileLocation");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.Item", b =>
                {
                    b.OwnsOne("TableTopCrucible.Core.ValueTypes.FileHashKey", "FileKey3d", b1 =>
                        {
                            b1.Property<Guid>("ItemGuid")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .HasColumnType("TEXT");

                            b1.HasKey("ItemGuid");

                            b1.ToTable("Items");

                            b1.WithOwner()
                                .HasForeignKey("ItemGuid");
                        });

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

                    b.OwnsMany("TableTopCrucible.Core.ValueTypes.Tag", "Tags", b1 =>
                        {
                            b1.Property<Guid>("ItemGuid")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Value")
                                .HasColumnType("TEXT");

                            b1.HasKey("ItemGuid", "Id");

                            b1.ToTable("Tag");

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

                    b.Navigation("FileKey3d");

                    b.Navigation("Id");

                    b.Navigation("Name");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("TableTopCrucible.Infrastructure.Models.Entities.Item", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
