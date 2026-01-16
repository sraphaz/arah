using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddMediaSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Media Assets
        migrationBuilder.CreateTable(
            name: "media_assets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                MediaType = table.Column<int>(type: "integer", nullable: false),
                MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                WidthPx = table.Column<int>(type: "integer", nullable: true),
                HeightPx = table.Column<int>(type: "integer", nullable: true),
                Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DeletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_media_assets", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_media_assets_UploadedByUserId",
            table: "media_assets",
            column: "UploadedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_media_assets_CreatedAtUtc",
            table: "media_assets",
            column: "CreatedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_media_assets_DeletedAtUtc",
            table: "media_assets",
            column: "DeletedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_media_assets_UploadedByUserId_DeletedAtUtc",
            table: "media_assets",
            columns: new[] { "UploadedByUserId", "DeletedAtUtc" });

        // Media Attachments
        migrationBuilder.CreateTable(
            name: "media_attachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MediaAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                OwnerType = table.Column<int>(type: "integer", nullable: false),
                OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_media_attachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_media_attachments_media_assets_MediaAssetId",
                    column: x => x.MediaAssetId,
                    principalTable: "media_assets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_media_attachments_OwnerType_OwnerId",
            table: "media_attachments",
            columns: new[] { "OwnerType", "OwnerId" });

        migrationBuilder.CreateIndex(
            name: "IX_media_attachments_MediaAssetId",
            table: "media_attachments",
            column: "MediaAssetId");

        migrationBuilder.CreateIndex(
            name: "IX_media_attachments_OwnerType_OwnerId_DisplayOrder",
            table: "media_attachments",
            columns: new[] { "OwnerType", "OwnerId", "DisplayOrder" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "media_attachments");

        migrationBuilder.DropTable(
            name: "media_assets");
    }
}