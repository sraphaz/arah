using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddNotificationConfig : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "notification_configs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                NotificationTypesJson = table.Column<string>(type: "jsonb", nullable: false),
                AvailableChannelsJson = table.Column<string>(type: "jsonb", nullable: false),
                TemplatesJson = table.Column<string>(type: "jsonb", nullable: false),
                DefaultChannelsJson = table.Column<string>(type: "jsonb", nullable: false),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_notification_configs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_notification_configs_TerritoryId",
            table: "notification_configs",
            column: "TerritoryId",
            unique: true,
            filter: "\"TerritoryId\" IS NOT NULL");

        // Índice único para config global (TerritoryId IS NULL)
        migrationBuilder.Sql(@"
            CREATE UNIQUE INDEX IX_notification_configs_Global 
            ON notification_configs (Id) 
            WHERE ""TerritoryId"" IS NULL;
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "notification_configs");
    }
}
