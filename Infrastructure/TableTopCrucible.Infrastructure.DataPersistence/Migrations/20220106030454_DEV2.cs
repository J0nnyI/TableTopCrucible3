using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.AddColumn<string>(
                name: "RawTags",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawTags",
                table: "Items");

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
        }
    }
}
