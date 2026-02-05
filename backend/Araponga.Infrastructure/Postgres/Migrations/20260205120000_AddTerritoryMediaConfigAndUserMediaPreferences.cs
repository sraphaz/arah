using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <summary>
/// Adiciona tabelas territory_media_configs e user_media_preferences para repositórios Postgres de mídia.
/// </summary>
public partial class AddTerritoryMediaConfigAndUserMediaPreferences : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "territory_media_configs",
            columns: table => new
            {
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                PostsJson = table.Column<string>(type: "jsonb", nullable: false),
                EventsJson = table.Column<string>(type: "jsonb", nullable: false),
                MarketplaceJson = table.Column<string>(type: "jsonb", nullable: false),
                ChatJson = table.Column<string>(type: "jsonb", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_media_configs", x => x.TerritoryId);
            });

        migrationBuilder.CreateTable(
            name: "user_media_preferences",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                ShowImages = table.Column<bool>(type: "boolean", nullable: false),
                ShowVideos = table.Column<bool>(type: "boolean", nullable: false),
                ShowAudio = table.Column<bool>(type: "boolean", nullable: false),
                AutoPlayVideos = table.Column<bool>(type: "boolean", nullable: false),
                AutoPlayAudio = table.Column<bool>(type: "boolean", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_media_preferences", x => x.UserId);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "territory_media_configs");
        migrationBuilder.DropTable(name: "user_media_preferences");
    }
}
