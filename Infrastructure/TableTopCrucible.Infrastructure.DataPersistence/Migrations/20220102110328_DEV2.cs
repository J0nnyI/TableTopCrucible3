using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "DirectorySetups");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "DirectorySetups",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DirectorySetups",
                newName: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "DirectorySetups",
                column: "Guid");
        }
    }
}
