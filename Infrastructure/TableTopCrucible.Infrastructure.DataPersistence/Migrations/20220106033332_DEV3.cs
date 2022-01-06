using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FileKey3d_Value",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id_Guid",
                table: "Items");

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "Items",
                column: "Guid");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemId");

            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "FileKey3d_Value",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Guid");
        }
    }
}
