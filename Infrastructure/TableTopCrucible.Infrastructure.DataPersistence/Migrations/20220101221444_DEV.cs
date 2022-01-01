using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectorySetups",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectorySetups", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    HashKey3d_Hash = table.Column<byte[]>(type: "BLOB", nullable: true),
                    HashKey3d_FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    FileKey3d_Raw = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: true),
                    Id_Guid = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Guid);
                    table.UniqueConstraint("AK_Items_FileKey3d_Raw", x => x.FileKey3d_Raw);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileLocation = table.Column<string>(type: "TEXT", nullable: true),
                    HashKey_Hash = table.Column<byte[]>(type: "BLOB", nullable: true),
                    HashKey_FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    HashKey_Raw = table.Column<string>(type: "TEXT", nullable: true),
                    LastWrite = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Id_Value = table.Column<Guid>(type: "TEXT", nullable: true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Files_Items_HashKey_Raw",
                        column: x => x.HashKey_Raw,
                        principalTable: "Items",
                        principalColumn: "FileKey3d_Raw",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    ItemGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => new { x.ItemGuid, x.Id });
                    table.ForeignKey(
                        name: "FK_Tag_Items_ItemGuid",
                        column: x => x.ItemGuid,
                        principalTable: "Items",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_HashKey_Raw",
                table: "Files",
                column: "HashKey_Raw");

            migrationBuilder.CreateIndex(
                name: "IX_Items_FileKey3d_Raw",
                table: "Items",
                column: "FileKey3d_Raw");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectorySetups");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
