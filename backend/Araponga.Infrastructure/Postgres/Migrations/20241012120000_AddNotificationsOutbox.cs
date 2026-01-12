using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddNotificationsOutbox : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                LastError = table.Column<string>(type: "text", nullable: true),
                ProcessAfterUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_outbox_messages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "user_notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Kind = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DataJson = table.Column<string>(type: "jsonb", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReadAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                SourceOutboxId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_notifications", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_ProcessedAtUtc_ProcessAfterUtc",
            table: "outbox_messages",
            columns: new[] { "ProcessedAtUtc", "ProcessAfterUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_Type_ProcessedAtUtc",
            table: "outbox_messages",
            columns: new[] { "Type", "ProcessedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_user_notifications_CreatedAtUtc",
            table: "user_notifications",
            column: "CreatedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_user_notifications_UserId",
            table: "user_notifications",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_user_notifications_SourceOutboxId_UserId",
            table: "user_notifications",
            columns: new[] { "SourceOutboxId", "UserId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "outbox_messages");
        migrationBuilder.DropTable(name: "user_notifications");
    }
}
