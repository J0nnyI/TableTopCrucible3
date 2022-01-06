using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups");

            migrationBuilder.RenameTable(
                name: "DirectorySetups",
                newName: "Directories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Directories",
                table: "Directories",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Directories",
                table: "Directories");

            migrationBuilder.RenameTable(
                name: "Directories",
                newName: "DirectorySetups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectorySetups",
                table: "DirectorySetups",
                column: "Id");
        }
    }
}
