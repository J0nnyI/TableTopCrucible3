using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DE6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id_Guid",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "RawTags",
                table: "Items",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "ItemIdRaw",
                table: "Images",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ItemIdRaw",
                table: "Images",
                newName: "IX_Images_ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Items",
                newName: "RawTags");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Images",
                newName: "ItemIdRaw");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ItemId",
                table: "Images",
                newName: "IX_Images_ItemIdRaw");

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
        }
    }
}
