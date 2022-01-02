using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups");

            migrationBuilder.RenameColumn(
                name: "Path_Value",
                table: "DirectorySetups",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "Name_Value",
                table: "DirectorySetups",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "DirectorySetups",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "DirectorySetups");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "DirectorySetups",
                newName: "Path_Value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DirectorySetups",
                newName: "Name_Value");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups",
                column: "Guid");
        }
    }
}
