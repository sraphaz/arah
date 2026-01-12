using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddAssetsAndPostAssets : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "territory_assets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ArchivedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ArchiveReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_assets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "asset_geo_anchors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_asset_geo_anchors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "asset_validations",
            columns: table => new
            {
                AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_asset_validations", x => new { x.AssetId, x.UserId });
            });

        migrationBuilder.CreateTable(
            name: "post_assets",
            columns: table => new
            {
                PostId = table.Column<Guid>(type: "uuid", nullable: false),
                AssetId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_post_assets", x => new { x.PostId, x.AssetId });
            });

        migrationBuilder.CreateIndex(
            name: "IX_territory_assets_TerritoryId",
            table: "territory_assets",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_assets_TerritoryId_Status",
            table: "territory_assets",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_territory_assets_TerritoryId_Type",
            table: "territory_assets",
            columns: new[] { "TerritoryId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_asset_geo_anchors_AssetId",
            table: "asset_geo_anchors",
            column: "AssetId");

        migrationBuilder.CreateIndex(
            name: "IX_asset_validations_UserId",
            table: "asset_validations",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_post_assets_AssetId",
            table: "post_assets",
            column: "AssetId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "post_assets");
        migrationBuilder.DropTable(name: "asset_validations");
        migrationBuilder.DropTable(name: "asset_geo_anchors");
        migrationBuilder.DropTable(name: "territory_assets");
    }
}
