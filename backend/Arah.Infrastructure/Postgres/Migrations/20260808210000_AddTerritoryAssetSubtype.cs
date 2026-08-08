using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arah.Infrastructure.Postgres.Migrations
{
    /// <summary>
    /// Migration hand-written (mesmo padrão de MakeUserEmailNullableAndUserDeviceFk).
    /// Sem *.Designer.cs — o modelo autoritativo está em ArahDbContextModelSnapshot.
    /// </summary>
    public partial class AddTerritoryAssetSubtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subtype",
                table: "territory_assets",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtype",
                table: "territory_assets");
        }
    }
}
