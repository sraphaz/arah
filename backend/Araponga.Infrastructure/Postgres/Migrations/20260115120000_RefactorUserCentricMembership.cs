using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class RefactorUserCentricMembership : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Adicionar campos de verificação de identidade ao User
        migrationBuilder.AddColumn<int>(
            name: "IdentityVerificationStatus",
            table: "users",
            type: "integer",
            nullable: false,
            defaultValue: 1); // Unverified = 1

        migrationBuilder.AddColumn<DateTime>(
            name: "IdentityVerifiedAtUtc",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);

        // 2. Criar tabela membership_settings
        migrationBuilder.CreateTable(
            name: "membership_settings",
            columns: table => new
            {
                membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                marketplace_opt_in = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_membership_settings", x => x.membership_id);
                table.ForeignKey(
                    name: "FK_membership_settings_territory_memberships_membership_id",
                    column: x => x.membership_id,
                    principalTable: "territory_memberships",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_membership_settings_membership_id",
            table: "membership_settings",
            column: "membership_id",
            unique: true);

        // 3. Criar tabela membership_capabilities
        migrationBuilder.CreateTable(
            name: "membership_capabilities",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                capability_type = table.Column<int>(type: "integer", nullable: false),
                granted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                granted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                granted_by_membership_id = table.Column<Guid>(type: "uuid", nullable: true),
                reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_membership_capabilities", x => x.id);
                table.ForeignKey(
                    name: "FK_membership_capabilities_territory_memberships_membership_id",
                    column: x => x.membership_id,
                    principalTable: "territory_memberships",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_membership_capabilities_membership_id",
            table: "membership_capabilities",
            column: "membership_id");

        migrationBuilder.CreateIndex(
            name: "IX_membership_capabilities_membership_id_capability_type",
            table: "membership_capabilities",
            columns: new[] { "membership_id", "capability_type" },
            filter: "\"revoked_at_utc\" IS NULL");

        // 4. Migrar ResidencyVerification de valores antigos para flags
        // Unverified (1) -> None (0)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 0
            WHERE ""ResidencyVerification"" = 1;
        ");

        // GeoVerified (2) -> GeoVerified (1)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 1
            WHERE ""ResidencyVerification"" = 2;
        ");

        // DocumentVerified (3) -> DocumentVerified (2)
        // Mas se já tem LastGeoVerifiedAtUtc, deve ser GeoVerified | DocumentVerified (3)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = CASE 
                WHEN ""LastGeoVerifiedAtUtc"" IS NOT NULL THEN 3  -- GeoVerified | DocumentVerified
                ELSE 2  -- Apenas DocumentVerified
            END
            WHERE ""ResidencyVerification"" = 3;
        ");

        // 5. Criar MembershipSettings para todos os Memberships existentes
        migrationBuilder.Sql(@"
            INSERT INTO membership_settings (membership_id, marketplace_opt_in, created_at_utc, updated_at_utc)
            SELECT 
                id AS membership_id,
                false AS marketplace_opt_in,
                created_at_utc AS created_at_utc,
                created_at_utc AS updated_at_utc
            FROM territory_memberships
            WHERE id NOT IN (SELECT membership_id FROM membership_settings);
        ");

        // 6. Criar MembershipCapability (Curator) para Users com UserRole.Curator
        // NOTA: Esta parte foi removida porque UserRole foi completamente removido do modelo.
        // Se houver necessidade de migrar capabilities de um sistema legado, isso deve ser feito
        // manualmente ou através de uma migration específica que leia de uma fonte externa.
        // migrationBuilder.Sql(@"..."); -- Removido: UserRole não existe mais
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Remover tabelas
        migrationBuilder.DropTable(name: "membership_capabilities");
        migrationBuilder.DropTable(name: "membership_settings");

        // Remover campos do User
        migrationBuilder.DropColumn(
            name: "IdentityVerificationStatus",
            table: "users");

        migrationBuilder.DropColumn(
            name: "IdentityVerifiedAtUtc",
            table: "users");

        // Reverter ResidencyVerification para valores antigos
        // None (0) -> Unverified (1)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 1
            WHERE ""ResidencyVerification"" = 0;
        ");

        // GeoVerified (1) -> GeoVerified (2)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 2
            WHERE ""ResidencyVerification"" = 1 AND ""LastGeoVerifiedAtUtc"" IS NOT NULL;
        ");

        // DocumentVerified (2) ou GeoVerified | DocumentVerified (3) -> DocumentVerified (3)
        migrationBuilder.Sql(@"
            UPDATE territory_memberships
            SET ""ResidencyVerification"" = 3
            WHERE ""ResidencyVerification"" IN (2, 3) AND ""LastDocumentVerifiedAtUtc"" IS NOT NULL;
        ");
    }
}
