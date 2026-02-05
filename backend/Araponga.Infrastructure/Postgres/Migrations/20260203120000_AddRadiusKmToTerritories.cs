using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <summary>
/// Adiciona RadiusKm ao território: raio do perímetro em km. Null = usar padrão do sistema (ex.: 5 km).
/// </summary>
public partial class AddRadiusKmToTerritories : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "RadiusKm",
            table: "territories",
            type: "double precision",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RadiusKm",
            table: "territories");
    }
}
