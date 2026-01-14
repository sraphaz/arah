using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddWorkItems : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "work_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RequiredSystemPermission = table.Column<int>(type: "integer", nullable: true),
                RequiredCapability = table.Column<int>(type: "integer", nullable: true),
                SubjectType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                PayloadJson = table.Column<string>(type: "text", nullable: true),
                Outcome = table.Column<int>(type: "integer", nullable: false),
                CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                CompletionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_work_items", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_work_items_Status",
            table: "work_items",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_Type",
            table: "work_items",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_TerritoryId_Status",
            table: "work_items",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_work_items_SubjectType_SubjectId",
            table: "work_items",
            columns: new[] { "SubjectType", "SubjectId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "work_items");
    }
}

