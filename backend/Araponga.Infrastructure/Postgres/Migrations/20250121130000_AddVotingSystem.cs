using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddVotingSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "votings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                OptionsJson = table.Column<string>(type: "text", nullable: false),
                Visibility = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_votings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "votes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                VotingId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                SelectedOption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_votes", x => x.Id);
                table.ForeignKey(
                    name: "FK_votes_votings_VotingId",
                    column: x => x.VotingId,
                    principalTable: "votings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_votings_TerritoryId",
            table: "votings",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_votings_CreatedByUserId",
            table: "votings",
            column: "CreatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_votings_Status",
            table: "votings",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_votings_TerritoryId_Status",
            table: "votings",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_votes_VotingId",
            table: "votes",
            column: "VotingId");

        migrationBuilder.CreateIndex(
            name: "IX_votes_UserId",
            table: "votes",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_votes_VotingId_UserId",
            table: "votes",
            columns: new[] { "VotingId", "UserId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "votes");

        migrationBuilder.DropTable(
            name: "votings");
    }
}
