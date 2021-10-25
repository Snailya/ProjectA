using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectA.Migrations
{
    public partial class MakeVersionIdAsDocVersionKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DocVersion",
                table: "DocVersion");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DocVersion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocVersion",
                table: "DocVersion",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocVersion_DocumentEntityId",
                table: "DocVersion",
                column: "DocumentEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DocVersion",
                table: "DocVersion");

            migrationBuilder.DropIndex(
                name: "IX_DocVersion_DocumentEntityId",
                table: "DocVersion");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DocVersion",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocVersion",
                table: "DocVersion",
                columns: new[] { "DocumentEntityId", "Id" });
        }
    }
}
