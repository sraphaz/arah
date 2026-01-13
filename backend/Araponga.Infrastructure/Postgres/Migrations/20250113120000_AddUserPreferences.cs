using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddUserPreferences : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "user_preferences",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileVisibility = table.Column<int>(type: "integer", nullable: false),
                ContactVisibility = table.Column<int>(type: "integer", nullable: false),
                ShareLocation = table.Column<bool>(type: "boolean", nullable: false),
                ShowMemberships = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsPostsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsCommentsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsEventsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsAlertsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsMarketplaceEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsModerationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                NotificationsMembershipRequestsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_preferences", x => x.UserId);
                table.ForeignKey(
                    name: "FK_user_preferences_users_UserId",
                    column: x => x.UserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_user_preferences_UserId",
            table: "user_preferences",
            column: "UserId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "user_preferences");
    }
}
