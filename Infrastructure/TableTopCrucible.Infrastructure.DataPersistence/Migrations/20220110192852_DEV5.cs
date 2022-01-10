using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class DEV5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_HashKeyRaw",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "FileLocation",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_HashKeyRaw",
                table: "Images",
                column: "HashKeyRaw");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_HashKeyRaw",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "FileLocation",
                table: "Files",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Images_HashKeyRaw",
                table: "Images",
                column: "HashKeyRaw",
                unique: true);
        }
    }
}
