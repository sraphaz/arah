using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddEmailPreferences : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "EmailReceiveEmails",
            table: "user_preferences",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<int>(
            name: "EmailFrequency",
            table: "user_preferences",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "EmailTypes",
            table: "user_preferences",
            type: "integer",
            nullable: false,
            defaultValue: 31);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EmailReceiveEmails",
            table: "user_preferences");

        migrationBuilder.DropColumn(
            name: "EmailFrequency",
            table: "user_preferences");

        migrationBuilder.DropColumn(
            name: "EmailTypes",
            table: "user_preferences");
    }
}
