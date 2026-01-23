using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddModerationRulesAndCharacterization : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "territory_moderation_rules",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByVotingId = table.Column<Guid>(type: "uuid", nullable: true),
                RuleType = table.Column<int>(type: "integer", nullable: false),
                RuleJson = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_moderation_rules", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "territory_characterizations",
            columns: table => new
            {
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                TagsJson = table.Column<string>(type: "text", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_characterizations", x => x.TerritoryId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_territory_moderation_rules_TerritoryId",
            table: "territory_moderation_rules",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_moderation_rules_CreatedByVotingId",
            table: "territory_moderation_rules",
            column: "CreatedByVotingId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_moderation_rules_TerritoryId_IsActive",
            table: "territory_moderation_rules",
            columns: new[] { "TerritoryId", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_territory_characterizations_TerritoryId",
            table: "territory_characterizations",
            column: "TerritoryId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "territory_moderation_rules");

        migrationBuilder.DropTable(
            name: "territory_characterizations");
    }
}
