using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_connections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RemovedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_connections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "connection_privacy_settings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WhoCanAddMe = table.Column<int>(type: "integer", nullable: false),
                    WhoCanSeeMyConnections = table.Column<int>(type: "integer", nullable: false),
                    ShowConnectionsInProfile = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connection_privacy_settings", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_RequesterUserId",
                table: "user_connections",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_Status",
                table: "user_connections",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_TargetUserId",
                table: "user_connections",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_RequesterUserId_TargetUserId",
                table: "user_connections",
                columns: new[] { "RequesterUserId", "TargetUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_connection_privacy_settings_UserId",
                table: "connection_privacy_settings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "user_connections");
            migrationBuilder.DropTable(name: "connection_privacy_settings");
        }
    }
}
