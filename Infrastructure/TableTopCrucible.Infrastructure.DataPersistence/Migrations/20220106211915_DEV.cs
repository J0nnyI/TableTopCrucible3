using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemId",
                columns: table => new
                {
                    ItemTempId2 = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemId", x => x.ItemTempId2);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    FileKey3d_Raw = table.Column<string>(type: "TEXT", nullable: false),
                    RawTags = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => x.Guid);
                    table.UniqueConstraint("AK_Items_FileKey3d_Raw", x => x.FileKey3d_Raw);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileLocation = table.Column<string>(type: "TEXT", nullable: true),
                    HashKey_Raw = table.Column<string>(type: "TEXT", nullable: true),
                    LastWrite = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Files_Items_HashKey_Raw",
                        column: x => x.HashKey_Raw,
                        principalTable: "Items",
                        principalColumn: "FileKey3d_Raw",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "FileKey3d",
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
                name: "Directories");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "ItemId");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
