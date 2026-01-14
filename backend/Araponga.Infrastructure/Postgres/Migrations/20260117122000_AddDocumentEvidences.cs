using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddDocumentEvidences : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "document_evidences",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                Kind = table.Column<int>(type: "integer", nullable: false),
                StorageProvider = table.Column<int>(type: "integer", nullable: false),
                StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                Sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                OriginalFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_document_evidences", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_document_evidences_UserId",
            table: "document_evidences",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_document_evidences_TerritoryId_Kind",
            table: "document_evidences",
            columns: new[] { "TerritoryId", "Kind" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "document_evidences");
    }
}

