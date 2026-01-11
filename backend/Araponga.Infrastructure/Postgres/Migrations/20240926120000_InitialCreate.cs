using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "active_territories",
            columns: table => new
            {
                SessionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_active_territories", x => x.SessionId);
            });

        migrationBuilder.CreateTable(
            name: "audit_entries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Action = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_entries", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "community_posts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Visibility = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                MapEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_community_posts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "feature_flags",
            columns: table => new
            {
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Flag = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_feature_flags", x => new { x.TerritoryId, x.Flag });
            });

        migrationBuilder.CreateTable(
            name: "health_alerts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_health_alerts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "map_entities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                Visibility = table.Column<int>(type: "integer", nullable: false),
                ConfirmationCount = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_map_entities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "map_entity_relations",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_map_entity_relations", x => new { x.UserId, x.EntityId });
            });

        migrationBuilder.CreateTable(
            name: "moderation_reports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                TargetType = table.Column<int>(type: "integer", nullable: false),
                TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                Reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_moderation_reports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "post_comments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PostId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_post_comments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "post_geo_anchors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PostId = table.Column<Guid>(type: "uuid", nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                Type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_post_geo_anchors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "post_likes",
            columns: table => new
            {
                PostId = table.Column<Guid>(type: "uuid", nullable: false),
                ActorId = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_post_likes", x => new { x.PostId, x.ActorId });
            });

        migrationBuilder.CreateTable(
            name: "post_shares",
            columns: table => new
            {
                PostId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_post_shares", x => new { x.PostId, x.UserId });
            });

        migrationBuilder.CreateTable(
            name: "sanctions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                Scope = table.Column<int>(type: "integer", nullable: false),
                TargetType = table.Column<int>(type: "integer", nullable: false),
                TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Reason = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_sanctions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "territories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                City = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                State = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "territory_memberships",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Role = table.Column<int>(type: "integer", nullable: false),
                VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_territory_memberships", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "user_blocks",
            columns: table => new
            {
                BlockerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                BlockedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_blocks", x => new { x.BlockerUserId, x.BlockedUserId });
            });

        migrationBuilder.CreateTable(
            name: "user_territories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_territories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                Cpf = table.Column<string>(type: "text", nullable: true),
                ForeignDocument = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                Address = table.Column<string>(type: "text", nullable: true),
                Provider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                ExternalId = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Role = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_active_territories_TerritoryId",
            table: "active_territories",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_audit_entries_ActorUserId",
            table: "audit_entries",
            column: "ActorUserId");

        migrationBuilder.CreateIndex(
            name: "IX_audit_entries_TerritoryId",
            table: "audit_entries",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_audit_entries_TimestampUtc",
            table: "audit_entries",
            column: "TimestampUtc");

        migrationBuilder.CreateIndex(
            name: "IX_community_posts_AuthorUserId",
            table: "community_posts",
            column: "AuthorUserId");

        migrationBuilder.CreateIndex(
            name: "IX_community_posts_MapEntityId",
            table: "community_posts",
            column: "MapEntityId");

        migrationBuilder.CreateIndex(
            name: "IX_community_posts_TerritoryId",
            table: "community_posts",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_community_posts_TerritoryId_CreatedAtUtc",
            table: "community_posts",
            columns: new[] { "TerritoryId", "CreatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_feature_flags_TerritoryId",
            table: "feature_flags",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_health_alerts_TerritoryId",
            table: "health_alerts",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_health_alerts_TerritoryId_Status",
            table: "health_alerts",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_map_entities_CreatedByUserId",
            table: "map_entities",
            column: "CreatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_map_entities_TerritoryId",
            table: "map_entities",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_map_entities_TerritoryId_Status",
            table: "map_entities",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_map_entities_TerritoryId_Visibility",
            table: "map_entities",
            columns: new[] { "TerritoryId", "Visibility" });

        migrationBuilder.CreateIndex(
            name: "IX_map_entity_relations_EntityId",
            table: "map_entity_relations",
            column: "EntityId");

        migrationBuilder.CreateIndex(
            name: "IX_moderation_reports_CreatedAtUtc",
            table: "moderation_reports",
            column: "CreatedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_moderation_reports_ReporterUserId",
            table: "moderation_reports",
            column: "ReporterUserId");

        migrationBuilder.CreateIndex(
            name: "IX_moderation_reports_TargetType_TargetId",
            table: "moderation_reports",
            columns: new[] { "TargetType", "TargetId" });

        migrationBuilder.CreateIndex(
            name: "IX_moderation_reports_TerritoryId_CreatedAtUtc",
            table: "moderation_reports",
            columns: new[] { "TerritoryId", "CreatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_post_comments_PostId",
            table: "post_comments",
            column: "PostId");

        migrationBuilder.CreateIndex(
            name: "IX_post_geo_anchors_PostId",
            table: "post_geo_anchors",
            column: "PostId");

        migrationBuilder.CreateIndex(
            name: "IX_post_likes_PostId",
            table: "post_likes",
            column: "PostId");

        migrationBuilder.CreateIndex(
            name: "IX_post_shares_PostId",
            table: "post_shares",
            column: "PostId");

        migrationBuilder.CreateIndex(
            name: "IX_sanctions_TargetId",
            table: "sanctions",
            column: "TargetId");

        migrationBuilder.CreateIndex(
            name: "IX_sanctions_TerritoryId",
            table: "sanctions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territories_City",
            table: "territories",
            column: "City");

        migrationBuilder.CreateIndex(
            name: "IX_territories_City_State",
            table: "territories",
            columns: new[] { "City", "State" });

        migrationBuilder.CreateIndex(
            name: "IX_territories_Name",
            table: "territories",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_territories_State",
            table: "territories",
            column: "State");

        migrationBuilder.CreateIndex(
            name: "IX_territory_memberships_TerritoryId",
            table: "territory_memberships",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_memberships_UserId",
            table: "territory_memberships",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_memberships_UserId_TerritoryId",
            table: "territory_memberships",
            columns: new[] { "UserId", "TerritoryId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_user_blocks_BlockerUserId",
            table: "user_blocks",
            column: "BlockerUserId");

        migrationBuilder.CreateIndex(
            name: "IX_user_territories_TerritoryId",
            table: "user_territories",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_user_territories_UserId",
            table: "user_territories",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_user_territories_UserId_TerritoryId",
            table: "user_territories",
            columns: new[] { "UserId", "TerritoryId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_users_Email",
            table: "users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_users_Provider_ExternalId",
            table: "users",
            columns: new[] { "Provider", "ExternalId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "active_territories");
        migrationBuilder.DropTable(name: "audit_entries");
        migrationBuilder.DropTable(name: "community_posts");
        migrationBuilder.DropTable(name: "feature_flags");
        migrationBuilder.DropTable(name: "health_alerts");
        migrationBuilder.DropTable(name: "map_entities");
        migrationBuilder.DropTable(name: "map_entity_relations");
        migrationBuilder.DropTable(name: "moderation_reports");
        migrationBuilder.DropTable(name: "post_comments");
        migrationBuilder.DropTable(name: "post_geo_anchors");
        migrationBuilder.DropTable(name: "post_likes");
        migrationBuilder.DropTable(name: "post_shares");
        migrationBuilder.DropTable(name: "sanctions");
        migrationBuilder.DropTable(name: "territories");
        migrationBuilder.DropTable(name: "territory_memberships");
        migrationBuilder.DropTable(name: "user_blocks");
        migrationBuilder.DropTable(name: "user_territories");
        migrationBuilder.DropTable(name: "users");
    }
}
