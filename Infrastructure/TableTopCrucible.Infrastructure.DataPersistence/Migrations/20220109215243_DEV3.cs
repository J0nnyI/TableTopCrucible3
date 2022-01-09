using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Items_HashKey_Raw",
                table: "Files");

            migrationBuilder.DropTable(
                name: "ItemId");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Items_FileKey3d_Raw",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_FileKey3d_Raw",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FileKey3d_Raw",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "HashKey_Raw",
                table: "Files",
                newName: "HashKeyRaw");

            migrationBuilder.RenameIndex(
                name: "FileKey3d",
                table: "Files",
                newName: "HashKey");

            migrationBuilder.AddColumn<string>(
                name: "FileKey3dRaw",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id_Guid",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    HashKeyRaw = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => x.Guid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_FileKey3dRaw",
                table: "Items",
                column: "FileKey3dRaw",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_HashKeyRaw",
                table: "Images",
                column: "HashKeyRaw",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Items_FileKey3dRaw",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FileKey3dRaw",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id_Guid",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "HashKeyRaw",
                table: "Files",
                newName: "HashKey_Raw");

            migrationBuilder.RenameIndex(
                name: "HashKey",
                table: "Files",
                newName: "FileKey3d");

            migrationBuilder.AddColumn<string>(
                name: "FileKey3d_Raw",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Items_FileKey3d_Raw",
                table: "Items",
                column: "FileKey3d_Raw");

            migrationBuilder.CreateTable(
                name: "ItemId",
                columns: table => new
                {
                    ItemTempId2 = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemId", x => x.ItemTempId2);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_FileKey3d_Raw",
                table: "Items",
                column: "FileKey3d_Raw");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Items_HashKey_Raw",
                table: "Files",
                column: "HashKey_Raw",
                principalTable: "Items",
                principalColumn: "FileKey3d_Raw",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
