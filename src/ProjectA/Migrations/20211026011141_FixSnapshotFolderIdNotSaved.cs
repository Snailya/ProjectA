using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectA.Migrations
{
    public partial class FixSnapshotFolderIdNotSaved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SnapshotFolderId",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapshotFolderId",
                table: "Documents");
        }
    }
}
