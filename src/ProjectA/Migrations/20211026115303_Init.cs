using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectA.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnapshotEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotFolderId = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionNumber_Major = table.Column<int>(type: "INTEGER", nullable: true),
                    VersionNumber_Minor = table.Column<int>(type: "INTEGER", nullable: true),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocVersion_Documents_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Documents",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SnapshotEntityId",
                table: "Documents",
                column: "SnapshotEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DocVersion_EntityId",
                table: "DocVersion",
                column: "EntityId");
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
