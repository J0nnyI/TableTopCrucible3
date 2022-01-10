using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemIdRaw",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Images_ItemIdRaw",
                table: "Images",
                column: "ItemIdRaw");

            migrationBuilder.AddForeignKey(
                name: "ItemId",
                table: "Images",
                column: "ItemIdRaw",
                principalTable: "Items",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ItemId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ItemIdRaw",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ItemIdRaw",
                table: "Images");
        }
    }
}
