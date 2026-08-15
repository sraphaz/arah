using Arah.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arah.Infrastructure.Postgres.Migrations
{
    /// <summary>
    /// Migration hand-written (mesmo padrão de AddTerritoryAssetSubtype).
    /// NaturalAsset ponto (WA-N1 / FASE24.0a).
    /// [Migration]/[DbContext] são obrigatórios para EF descobrir a migration (sem Designer).
    /// </summary>
    [DbContext(typeof(ArahDbContext))]
    [Migration("20260810180000_AddNaturalAssets")]
    public partial class AddNaturalAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "natural_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    WaterType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PotabilityNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastTestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_natural_assets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_natural_assets_TerritoryId",
                table: "natural_assets",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_natural_assets_TerritoryId_Status",
                table: "natural_assets",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_natural_assets_TerritoryId_Type",
                table: "natural_assets",
                columns: new[] { "TerritoryId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "natural_assets");
        }
    }
}
