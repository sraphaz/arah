using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddTerritoryJoinRequests : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "territory_join_requests",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                RequesterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Message = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DecidedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DecidedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_join_requests", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "territory_join_request_recipients",
            columns: table => new
            {
                JoinRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                RecipientUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RespondedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_join_request_recipients", x => new { x.JoinRequestId, x.RecipientUserId });
            });

        migrationBuilder.CreateIndex(
            name: "IX_territory_join_requests_TerritoryId",
            table: "territory_join_requests",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_join_requests_RequesterUserId",
            table: "territory_join_requests",
            column: "RequesterUserId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_join_requests_TerritoryId_RequesterUserId",
            table: "territory_join_requests",
            columns: new[] { "TerritoryId", "RequesterUserId" },
            unique: true,
            filter: "\"Status\" = 'Pending'");

        migrationBuilder.CreateIndex(
            name: "IX_territory_join_request_recipients_JoinRequestId",
            table: "territory_join_request_recipients",
            column: "JoinRequestId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_join_request_recipients_RecipientUserId",
            table: "territory_join_request_recipients",
            column: "RecipientUserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "territory_join_request_recipients");

        migrationBuilder.DropTable(
            name: "territory_join_requests");
    }
}
