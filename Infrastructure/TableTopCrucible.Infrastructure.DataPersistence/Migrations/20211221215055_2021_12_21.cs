using Microsoft.EntityFrameworkCore.Migrations;

namespace TableTopCrucible.Infrastructure.DataPersistence.Migrations
{
    public partial class _2021_12_21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Items_ItemGuid",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "ItemGuid",
                table: "Files",
                newName: "HashKey_Raw");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ItemGuid",
                table: "Files",
                newName: "IX_Files_HashKey_Raw");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Items_HashKey_Raw",
                table: "Files");

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
                newName: "ItemGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Files_HashKey_Raw",
                table: "Files",
                newName: "IX_Files_ItemGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Items_ItemGuid",
                table: "Files",
                column: "ItemGuid",
                principalTable: "Items",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
