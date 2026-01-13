using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddTerritoryEvents : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ReferenceType",
            table: "community_posts",
            type: "character varying(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "ReferenceId",
            table: "community_posts",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "territory_events",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                LocationLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByMembership = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_events", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "event_participations",
            columns: table => new
            {
                EventId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_event_participations", x => new { x.EventId, x.UserId });
            });

        migrationBuilder.CreateIndex(
            name: "IX_community_posts_ReferenceType_ReferenceId",
            table: "community_posts",
            columns: new[] { "ReferenceType", "ReferenceId" });

        migrationBuilder.CreateIndex(
            name: "IX_territory_events_TerritoryId_StartsAtUtc",
            table: "territory_events",
            columns: new[] { "TerritoryId", "StartsAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_territory_events_TerritoryId_Status",
            table: "territory_events",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_territory_events_Latitude_Longitude",
            table: "territory_events",
            columns: new[] { "Latitude", "Longitude" });

        migrationBuilder.CreateIndex(
            name: "IX_event_participations_EventId_Status",
            table: "event_participations",
            columns: new[] { "EventId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_event_participations_UserId_Status",
            table: "event_participations",
            columns: new[] { "UserId", "Status" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "event_participations");
        migrationBuilder.DropTable(name: "territory_events");

        migrationBuilder.DropIndex(
            name: "IX_community_posts_ReferenceType_ReferenceId",
            table: "community_posts");

        migrationBuilder.DropColumn(
            name: "ReferenceType",
            table: "community_posts");

        migrationBuilder.DropColumn(
            name: "ReferenceId",
            table: "community_posts");
    }
}
