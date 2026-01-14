using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class RenameProviderToAuthProvider : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Renomear coluna provider para auth_provider
        migrationBuilder.RenameColumn(
            name: "Provider",
            table: "users",
            newName: "auth_provider");

        // Renomear índice único composto
        migrationBuilder.DropIndex(
            name: "IX_users_Provider_ExternalId",
            table: "users");

        migrationBuilder.CreateIndex(
            name: "IX_users_auth_provider_external_id",
            table: "users",
            columns: new[] { "auth_provider", "ExternalId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverter: renomear auth_provider de volta para Provider
        migrationBuilder.DropIndex(
            name: "IX_users_auth_provider_external_id",
            table: "users");

        migrationBuilder.RenameColumn(
            name: "auth_provider",
            table: "users",
            newName: "Provider");

        migrationBuilder.CreateIndex(
            name: "IX_users_Provider_ExternalId",
            table: "users",
            columns: new[] { "Provider", "ExternalId" },
            unique: true);
    }
}
