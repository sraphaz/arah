using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddUserAvatarAndBio : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "AvatarMediaAssetId",
            table: "users",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Bio",
            table: "users",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_users_AvatarMediaAssetId",
            table: "users",
            column: "AvatarMediaAssetId");

        migrationBuilder.AddForeignKey(
            name: "FK_users_media_assets_AvatarMediaAssetId",
            table: "users",
            column: "AvatarMediaAssetId",
            principalTable: "media_assets",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_users_media_assets_AvatarMediaAssetId",
            table: "users");

        migrationBuilder.DropIndex(
            name: "IX_users_AvatarMediaAssetId",
            table: "users");

        migrationBuilder.DropColumn(
            name: "AvatarMediaAssetId",
            table: "users");

        migrationBuilder.DropColumn(
            name: "Bio",
            table: "users");
    }
}
