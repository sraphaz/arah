using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arah.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeSplitRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "territory_memberships",
                type: "bytea",
                nullable: false,
                defaultValueSql: "gen_random_bytes(8)",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "community_posts",
                type: "bytea",
                nullable: false,
                defaultValueSql: "gen_random_bytes(8)",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.CreateTable(
                name: "fee_split_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ImplementerSharePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TerritoryFundSharePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    PlatformSharePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    EffectiveFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SupersededAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fee_split_rules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fee_split_rules_TerritoryId_RevenueType",
                table: "fee_split_rules",
                columns: new[] { "TerritoryId", "RevenueType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fee_split_rules");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "territory_memberships",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldDefaultValueSql: "gen_random_bytes(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "community_posts",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldDefaultValueSql: "gen_random_bytes(8)");
        }
    }
}
