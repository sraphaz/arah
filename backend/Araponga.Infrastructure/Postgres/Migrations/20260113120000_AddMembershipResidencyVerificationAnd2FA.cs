using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddMembershipResidencyVerificationAnd2FA : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Adicionar novos campos em territory_memberships
        migrationBuilder.AddColumn<int>(
            name: "ResidencyVerification",
            table: "territory_memberships",
            type: "integer",
            nullable: false,
            defaultValue: 1); // Unverified = 1

        migrationBuilder.AddColumn<DateTime>(
            name: "LastGeoVerifiedAtUtc",
            table: "territory_memberships",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastDocumentVerifiedAtUtc",
            table: "territory_memberships",
            type: "timestamp with time zone",
            nullable: true);

        // Migrar dados existentes: VerificationStatus -> ResidencyVerification
        // Pending + Resident -> Unverified
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 1
            WHERE ""VerificationStatus"" = 0 AND ""Role"" = 1;
        ");

        // Validated + Resident -> GeoVerified (assumir geo como padrão)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 2,
                ""LastGeoVerifiedAtUtc"" = ""CreatedAtUtc""
            WHERE ""VerificationStatus"" = 1 AND ""Role"" = 1;
        ");

        // Rejected + Resident -> Unverified
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 1
            WHERE ""VerificationStatus"" = 2 AND ""Role"" = 1;
        ");

        // Visitor sempre Unverified
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 1
            WHERE ""Role"" = 0;
        ");

        // Criar índice único parcial para garantir 1 Resident por User
        migrationBuilder.CreateIndex(
            name: "IX_territory_memberships_UserId_Resident",
            table: "territory_memberships",
            column: "UserId",
            unique: true,
            filter: @"""Role"" = 1");

        // Adicionar campos 2FA em users
        migrationBuilder.AddColumn<bool>(
            name: "TwoFactorEnabled",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "TwoFactorSecret",
            table: "users",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TwoFactorRecoveryCodesHash",
            table: "users",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "TwoFactorVerifiedAtUtc",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Remover campos 2FA
        migrationBuilder.DropColumn(
            name: "TwoFactorEnabled",
            table: "users");

        migrationBuilder.DropColumn(
            name: "TwoFactorSecret",
            table: "users");

        migrationBuilder.DropColumn(
            name: "TwoFactorRecoveryCodesHash",
            table: "users");

        migrationBuilder.DropColumn(
            name: "TwoFactorVerifiedAtUtc",
            table: "users");

        // Remover índice único parcial
        migrationBuilder.DropIndex(
            name: "IX_territory_memberships_UserId_Resident",
            table: "territory_memberships");

        // Remover novos campos de territory_memberships
        migrationBuilder.DropColumn(
            name: "ResidencyVerification",
            table: "territory_memberships");

        migrationBuilder.DropColumn(
            name: "LastGeoVerifiedAtUtc",
            table: "territory_memberships");

        migrationBuilder.DropColumn(
            name: "LastDocumentVerifiedAtUtc",
            table: "territory_memberships");
    }
}
