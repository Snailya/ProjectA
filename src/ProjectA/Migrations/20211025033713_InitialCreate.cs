using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectA.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnapshotEntityId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Documents_Documents_SnapshotEntityId",
                        column: x => x.SnapshotEntityId,
                        principalTable: "Documents",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocVersion",
                columns: table => new
                {
                    DocumentEntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionNumber_Major = table.Column<int>(type: "INTEGER", nullable: true),
                    VersionNumber_Minor = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocVersion", x => new { x.DocumentEntityId, x.Id });
                    table.ForeignKey(
                        name: "FK_DocVersion_Documents_DocumentEntityId",
                        column: x => x.DocumentEntityId,
                        principalTable: "Documents",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SnapshotEntityId",
                table: "Documents",
                column: "SnapshotEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocVersion");

            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
