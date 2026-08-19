using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arah.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddTerritoryFiscalConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "territory_fiscal_pack_bindings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PackId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ActivatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MunicipalityIbge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_fiscal_pack_bindings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_payment_methods_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodsCsv = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PspProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_payment_methods_configs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_territory_fiscal_pack_bindings_TerritoryId",
                table: "territory_fiscal_pack_bindings",
                column: "TerritoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_territory_payment_methods_configs_TerritoryId",
                table: "territory_payment_methods_configs",
                column: "TerritoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "territory_fiscal_pack_bindings");

            migrationBuilder.DropTable(
                name: "territory_payment_methods_configs");
        }
    }
}
