using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddUserInterests : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "user_interests",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                InterestTag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_interests", x => x.Id);
                table.ForeignKey(
                    name: "FK_user_interests_users_UserId",
                    column: x => x.UserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_user_interests_UserId",
            table: "user_interests",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_user_interests_InterestTag",
            table: "user_interests",
            column: "InterestTag");

        migrationBuilder.CreateIndex(
            name: "IX_user_interests_UserId_InterestTag",
            table: "user_interests",
            columns: new[] { "UserId", "InterestTag" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "user_interests");
    }
}
