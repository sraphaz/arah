using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedInitialCreate : Migration
    {
        /// <inheritdoc />
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
                name: "asset_geo_anchors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_geo_anchors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "asset_validations",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_validations", x => new { x.AssetId, x.UserId });
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
                name: "cart_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chat_conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LockedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisabledAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "checkout_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    TitleSnapshot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    LineSubtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PlatformFeeLine = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkout_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "checkouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ItemsSubtotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PlatformFeeAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkouts", x => x.Id);
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
                    ReferenceType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EditCount = table.Column<int>(type: "integer", nullable: false),
                    TagsJson = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_posts", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxUses = table.Column<int>(type: "integer", nullable: true),
                    UsedCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StripeCouponId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_evidences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    StorageProvider = table.Column<int>(type: "integer", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_evidences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "email_queue_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_html = table.Column<bool>(type: "boolean", nullable: false),
                    template_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    template_data_json = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    scheduled_for = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    next_retry_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_queue_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "event_participations",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participations", x => new { x.EventId, x.UserId });
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
                name: "financial_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedTransactionIdsJson = table.Column<string>(type: "text", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_transactions", x => x.Id);
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
                name: "item_inquiries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_inquiries", x => x.Id);
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
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "media_storage_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    SettingsJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_storage_configs", x => x.Id);
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
                name: "notification_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    NotificationTypesJson = table.Column<string>(type: "jsonb", nullable: false),
                    AvailableChannelsJson = table.Column<string>(type: "jsonb", nullable: false),
                    TemplatesJson = table.Column<string>(type: "jsonb", nullable: false),
                    DefaultChannelsJson = table.Column<string>(type: "jsonb", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_configs", x => x.Id);
                });

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
                name: "platform_expense_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayoutAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PayoutId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FinancialTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_expense_transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platform_fee_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    FeeMode = table.Column<int>(type: "integer", nullable: false),
                    FeeValue = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_fee_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platform_financial_balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalRevenueInCents = table.Column<long>(type: "bigint", nullable: false),
                    TotalExpensesInCents = table.Column<long>(type: "bigint", nullable: false),
                    NetBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_financial_balances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platform_revenue_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeeAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FinancialTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_revenue_transactions", x => x.Id);
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
                name: "privacy_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredRoles = table.Column<string>(type: "jsonb", nullable: true),
                    RequiredCapabilities = table.Column<string>(type: "jsonb", nullable: true),
                    RequiredSystemPermissions = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_privacy_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "privacy_policy_acceptances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrivacyPolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcceptedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AcceptedVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_privacy_policy_acceptances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "reconciliation_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReconciliationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    ActualAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    DifferenceInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReconciledByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reconciliation_records", x => x.Id);
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
                name: "seller_balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PendingAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    ReadyForPayoutAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    PaidAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_balances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seller_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrossAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    PlatformFeeInCents = table.Column<long>(type: "bigint", nullable: false),
                    NetAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PayoutId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReadyForPayoutAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinancialTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_item_ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_item_ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Tags = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PricingType = table.Column<int>(type: "integer", nullable: false),
                    PriceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Unit = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_rating_responses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RatingId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_rating_responses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StripeInvoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StripePaymentIntentId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeType = table.Column<int>(type: "integer", nullable: false),
                    PreviousStateJson = table.Column<string>(type: "jsonb", nullable: true),
                    NewStateJson = table.Column<string>(type: "jsonb", nullable: true),
                    ChangeReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plan_histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    PricePerCycle = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    BillingCycle = table.Column<int>(type: "integer", nullable: true),
                    CapabilitiesJson = table.Column<string>(type: "jsonb", nullable: false),
                    LimitsJson = table.Column<string>(type: "jsonb", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    TrialDays = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StripePriceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StripeProductId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrialStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrialEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelAtPeriodEnd = table.Column<bool>(type: "boolean", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "system_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "terms_acceptances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermsOfServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcceptedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AcceptedVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terms_acceptances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "terms_of_services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredRoles = table.Column<string>(type: "jsonb", nullable: true),
                    RequiredCapabilities = table.Column<string>(type: "jsonb", nullable: true),
                    RequiredSystemPermissions = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terms_of_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentTerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    State = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RadiusKm = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchivedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchiveReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_characterizations",
                columns: table => new
                {
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsJson = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_characterizations", x => x.TerritoryId);
                });

            migrationBuilder.CreateTable(
                name: "territory_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    LocationLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByMembership = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_events", x => x.Id);
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
                name: "territory_media_configs",
                columns: table => new
                {
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostsJson = table.Column<string>(type: "jsonb", nullable: false),
                    EventsJson = table.Column<string>(type: "jsonb", nullable: false),
                    MarketplaceJson = table.Column<string>(type: "jsonb", nullable: false),
                    ChatJson = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_media_configs", x => x.TerritoryId);
                });

            migrationBuilder.CreateTable(
                name: "territory_memberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    ResidencyVerification = table.Column<int>(type: "integer", nullable: false),
                    LastGeoVerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastDocumentVerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_memberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_moderation_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByVotingId = table.Column<Guid>(type: "uuid", nullable: true),
                    RuleType = table.Column<int>(type: "integer", nullable: false),
                    RuleJson = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_moderation_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_payout_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RetentionPeriodDays = table.Column<int>(type: "integer", nullable: false),
                    MinimumPayoutAmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    MaximumPayoutAmountInCents = table.Column<long>(type: "bigint", nullable: true),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    AutoPayoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_payout_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "territory_stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ContactVisibility = table.Column<int>(type: "integer", nullable: false),
                    Phone = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PreferredContactMethod = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_territory_stores", x => x.Id);
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
                name: "user_devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RegisteredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUsedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_media_preferences",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShowImages = table.Column<bool>(type: "boolean", nullable: false),
                    ShowVideos = table.Column<bool>(type: "boolean", nullable: false),
                    ShowAudio = table.Column<bool>(type: "boolean", nullable: false),
                    AutoPlayVideos = table.Column<bool>(type: "boolean", nullable: false),
                    AutoPlayAudio = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_media_preferences", x => x.UserId);
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
                    AuthProvider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorSecret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TwoFactorRecoveryCodesHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TwoFactorVerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdentityVerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    IdentityVerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvatarMediaAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

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
                name: "work_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TerritoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequiredSystemPermission = table.Column<int>(type: "integer", nullable: true),
                    RequiredCapability = table.Column<int>(type: "integer", nullable: true),
                    SubjectType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: true),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chat_conversation_participants",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeftAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MutedUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastReadMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastReadAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_conversation_participants", x => new { x.ConversationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_chat_conversation_participants_chat_conversations_Conversat~",
                        column: x => x.ConversationId,
                        principalTable: "chat_conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_conversation_stats",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastMessageAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSenderUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastPreview = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MessageCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_conversation_stats", x => x.ConversationId);
                    table.ForeignKey(
                        name: "FK_chat_conversation_stats_chat_conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "chat_conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    PayloadJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "chat_conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_status_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_status_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transaction_status_histories_financial_transactions_Financi~",
                        column: x => x.FinancialTransactionId,
                        principalTable: "financial_transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "membership_capabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    CapabilityType = table.Column<int>(type: "integer", nullable: false),
                    GrantedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GrantedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    GrantedByMembershipId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_capabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_membership_capabilities_territory_memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "territory_memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "membership_settings",
                columns: table => new
                {
                    MembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketplaceOptIn = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_settings", x => x.MembershipId);
                    table.ForeignKey(
                        name: "FK_membership_settings_territory_memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "territory_memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "system_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionType = table.Column<int>(type: "integer", nullable: false),
                    GrantedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GrantedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_system_permissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    EmailReceiveEmails = table.Column<bool>(type: "boolean", nullable: false),
                    EmailFrequency = table.Column<int>(type: "integer", nullable: false),
                    EmailTypes = table.Column<int>(type: "integer", nullable: false),
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
                name: "IX_active_territories_TerritoryId",
                table: "active_territories",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_geo_anchors_AssetId",
                table: "asset_geo_anchors",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_validations_UserId",
                table: "asset_validations",
                column: "UserId");

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
                name: "IX_cart_items_CartId",
                table: "cart_items",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_CartId_ItemId",
                table: "cart_items",
                columns: new[] { "CartId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_ItemId",
                table: "cart_items",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_TerritoryId",
                table: "carts",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_TerritoryId_UserId",
                table: "carts",
                columns: new[] { "TerritoryId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_carts_UserId",
                table: "carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_conversation_participants_UserId",
                table: "chat_conversation_participants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_conversation_stats_LastMessageAtUtc",
                table: "chat_conversation_stats",
                column: "LastMessageAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_chat_conversations_TerritoryId",
                table: "chat_conversations",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_conversations_TerritoryId_Kind",
                table: "chat_conversations",
                columns: new[] { "TerritoryId", "Kind" },
                unique: true,
                filter: "\"TerritoryId\" IS NOT NULL AND \"Kind\" IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_chat_conversations_TerritoryId_Kind_Status_CreatedAtUtc",
                table: "chat_conversations",
                columns: new[] { "TerritoryId", "Kind", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ConversationId",
                table: "chat_messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ConversationId_CreatedAtUtc_Id",
                table: "chat_messages",
                columns: new[] { "ConversationId", "CreatedAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_SenderUserId_CreatedAtUtc",
                table: "chat_messages",
                columns: new[] { "SenderUserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_checkout_items_CheckoutId",
                table: "checkout_items",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_checkout_items_ItemId",
                table: "checkout_items",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_BuyerUserId",
                table: "checkouts",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_StoreId",
                table: "checkouts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_TerritoryId",
                table: "checkouts",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_AuthorUserId",
                table: "community_posts",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_MapEntityId",
                table: "community_posts",
                column: "MapEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_ReferenceType_ReferenceId",
                table: "community_posts",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_TagsJson",
                table: "community_posts",
                column: "TagsJson",
                filter: "\"TagsJson\" IS NOT NULL")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_TerritoryId",
                table: "community_posts",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_TerritoryId_CreatedAtUtc",
                table: "community_posts",
                columns: new[] { "TerritoryId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_community_posts_TerritoryId_Status_CreatedAtUtc",
                table: "community_posts",
                columns: new[] { "TerritoryId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_connection_privacy_settings_UserId",
                table: "connection_privacy_settings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupons_Code",
                table: "coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupons_IsActive",
                table: "coupons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_StripeCouponId",
                table: "coupons",
                column: "StripeCouponId",
                unique: true,
                filter: "\"StripeCouponId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_document_evidences_TerritoryId_Kind",
                table: "document_evidences",
                columns: new[] { "TerritoryId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_document_evidences_UserId",
                table: "document_evidences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_event_participations_EventId_Status",
                table: "event_participations",
                columns: new[] { "EventId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_event_participations_UserId_Status",
                table: "event_participations",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_feature_flags_TerritoryId",
                table: "feature_flags",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_transactions_RelatedEntityId",
                table: "financial_transactions",
                column: "RelatedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_transactions_TerritoryId",
                table: "financial_transactions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_transactions_TerritoryId_Status",
                table: "financial_transactions",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_financial_transactions_TerritoryId_Type",
                table: "financial_transactions",
                columns: new[] { "TerritoryId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_health_alerts_TerritoryId",
                table: "health_alerts",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_health_alerts_TerritoryId_Status",
                table: "health_alerts",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_item_inquiries_FromUserId",
                table: "item_inquiries",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_item_inquiries_ItemId",
                table: "item_inquiries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_item_inquiries_StoreId",
                table: "item_inquiries",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_item_inquiries_TerritoryId",
                table: "item_inquiries",
                column: "TerritoryId");

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
                name: "IX_media_assets_CreatedAtUtc",
                table: "media_assets",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_media_assets_DeletedAtUtc",
                table: "media_assets",
                column: "DeletedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_media_assets_UploadedByUserId",
                table: "media_assets",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_media_assets_UploadedByUserId_DeletedAtUtc",
                table: "media_assets",
                columns: new[] { "UploadedByUserId", "DeletedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_media_attachments_MediaAssetId",
                table: "media_attachments",
                column: "MediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_media_attachments_OwnerType_OwnerId",
                table: "media_attachments",
                columns: new[] { "OwnerType", "OwnerId" });

            migrationBuilder.CreateIndex(
                name: "IX_media_attachments_OwnerType_OwnerId_DisplayOrder",
                table: "media_attachments",
                columns: new[] { "OwnerType", "OwnerId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_media_storage_configs_IsActive",
                table: "media_storage_configs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_membership_capabilities_MembershipId",
                table: "membership_capabilities",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_membership_capabilities_MembershipId_CapabilityType",
                table: "membership_capabilities",
                columns: new[] { "MembershipId", "CapabilityType" },
                filter: "\"RevokedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_membership_settings_MembershipId",
                table: "membership_settings",
                column: "MembershipId",
                unique: true);

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
                name: "IX_moderation_reports_TargetType_TargetId_CreatedAtUtc",
                table: "moderation_reports",
                columns: new[] { "TargetType", "TargetId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_moderation_reports_TerritoryId_CreatedAtUtc",
                table: "moderation_reports",
                columns: new[] { "TerritoryId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_notification_configs_TerritoryId",
                table: "notification_configs",
                column: "TerritoryId",
                unique: true,
                filter: "\"TerritoryId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_ProcessedAtUtc_ProcessAfterUtc",
                table: "outbox_messages",
                columns: new[] { "ProcessedAtUtc", "ProcessAfterUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_Type_ProcessedAtUtc",
                table: "outbox_messages",
                columns: new[] { "Type", "ProcessedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_platform_expense_transactions_PayoutId",
                table: "platform_expense_transactions",
                column: "PayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_expense_transactions_SellerTransactionId",
                table: "platform_expense_transactions",
                column: "SellerTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_expense_transactions_TerritoryId",
                table: "platform_expense_transactions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_fee_configs_TerritoryId",
                table: "platform_fee_configs",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_fee_configs_TerritoryId_ItemType_IsActive",
                table: "platform_fee_configs",
                columns: new[] { "TerritoryId", "ItemType", "IsActive" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platform_financial_balances_TerritoryId",
                table: "platform_financial_balances",
                column: "TerritoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platform_revenue_transactions_CheckoutId",
                table: "platform_revenue_transactions",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_revenue_transactions_TerritoryId",
                table: "platform_revenue_transactions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_PostId",
                table: "post_comments",
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
                name: "IX_privacy_policies_EffectiveDate",
                table: "privacy_policies",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policies_IsActive",
                table: "privacy_policies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policies_Version",
                table: "privacy_policies",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policy_acceptances_PrivacyPolicyId",
                table: "privacy_policy_acceptances",
                column: "PrivacyPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policy_acceptances_UserId",
                table: "privacy_policy_acceptances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policy_acceptances_UserId_PrivacyPolicyId",
                table: "privacy_policy_acceptances",
                columns: new[] { "UserId", "PrivacyPolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_privacy_policy_acceptances_UserId_PrivacyPolicyId_IsRevoked",
                table: "privacy_policy_acceptances",
                columns: new[] { "UserId", "PrivacyPolicyId", "IsRevoked" });

            migrationBuilder.CreateIndex(
                name: "IX_reconciliation_records_ReconciliationDate",
                table: "reconciliation_records",
                column: "ReconciliationDate");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliation_records_TerritoryId",
                table: "reconciliation_records",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliation_records_TerritoryId_Status",
                table: "reconciliation_records",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_sanctions_TargetId",
                table: "sanctions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_sanctions_TerritoryId",
                table: "sanctions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_balances_SellerUserId",
                table: "seller_balances",
                column: "SellerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_balances_TerritoryId",
                table: "seller_balances",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_balances_TerritoryId_SellerUserId",
                table: "seller_balances",
                columns: new[] { "TerritoryId", "SellerUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seller_transactions_CheckoutId",
                table: "seller_transactions",
                column: "CheckoutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seller_transactions_PayoutId",
                table: "seller_transactions",
                column: "PayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_transactions_SellerUserId",
                table: "seller_transactions",
                column: "SellerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_transactions_TerritoryId",
                table: "seller_transactions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_seller_transactions_TerritoryId_Status",
                table: "seller_transactions",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_store_item_ratings_StoreItemId",
                table: "store_item_ratings",
                column: "StoreItemId");

            migrationBuilder.CreateIndex(
                name: "IX_store_item_ratings_StoreItemId_UserId",
                table: "store_item_ratings",
                columns: new[] { "StoreItemId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_item_ratings_UserId",
                table: "store_item_ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_store_items_StoreId",
                table: "store_items",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_store_items_TerritoryId",
                table: "store_items",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_store_items_TerritoryId_Type_Status",
                table: "store_items",
                columns: new[] { "TerritoryId", "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_store_rating_responses_RatingId",
                table: "store_rating_responses",
                column: "RatingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_rating_responses_StoreId",
                table: "store_rating_responses",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_store_ratings_StoreId",
                table: "store_ratings",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_store_ratings_StoreId_UserId",
                table: "store_ratings",
                columns: new[] { "StoreId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_ratings_UserId",
                table: "store_ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_coupons_CouponId",
                table: "subscription_coupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_coupons_SubscriptionId",
                table: "subscription_coupons",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_coupons_SubscriptionId_CouponId",
                table: "subscription_coupons",
                columns: new[] { "SubscriptionId", "CouponId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_Status",
                table: "subscription_payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_StripeInvoiceId",
                table: "subscription_payments",
                column: "StripeInvoiceId",
                unique: true,
                filter: "\"StripeInvoiceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_SubscriptionId",
                table: "subscription_payments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_histories_ChangedByUserId",
                table: "subscription_plan_histories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_histories_PlanId",
                table: "subscription_plan_histories",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_histories_PlanId_ChangedAtUtc",
                table: "subscription_plan_histories",
                columns: new[] { "PlanId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_Scope",
                table: "subscription_plans",
                column: "Scope");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_Scope_IsDefault",
                table: "subscription_plans",
                columns: new[] { "Scope", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_Scope_TerritoryId",
                table: "subscription_plans",
                columns: new[] { "Scope", "TerritoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_TerritoryId",
                table: "subscription_plans",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_PlanId",
                table: "subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_Status",
                table: "subscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_StripeSubscriptionId",
                table: "subscriptions",
                column: "StripeSubscriptionId",
                unique: true,
                filter: "\"StripeSubscriptionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_TerritoryId",
                table: "subscriptions",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_UserId",
                table: "subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_UserId_TerritoryId",
                table: "subscriptions",
                columns: new[] { "UserId", "TerritoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_system_configs_Category",
                table: "system_configs",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_system_configs_Key",
                table: "system_configs",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_system_permissions_PermissionType",
                table: "system_permissions",
                column: "PermissionType",
                filter: "\"RevokedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_system_permissions_UserId",
                table: "system_permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_system_permissions_UserId_PermissionType",
                table: "system_permissions",
                columns: new[] { "UserId", "PermissionType" },
                unique: true,
                filter: "\"RevokedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_terms_acceptances_TermsOfServiceId",
                table: "terms_acceptances",
                column: "TermsOfServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_terms_acceptances_UserId",
                table: "terms_acceptances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_terms_acceptances_UserId_TermsOfServiceId",
                table: "terms_acceptances",
                columns: new[] { "UserId", "TermsOfServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_terms_acceptances_UserId_TermsOfServiceId_IsRevoked",
                table: "terms_acceptances",
                columns: new[] { "UserId", "TermsOfServiceId", "IsRevoked" });

            migrationBuilder.CreateIndex(
                name: "IX_terms_of_services_EffectiveDate",
                table: "terms_of_services",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_terms_of_services_IsActive",
                table: "terms_of_services",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_terms_of_services_Version",
                table: "terms_of_services",
                column: "Version");

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
                name: "IX_territory_assets_TerritoryId",
                table: "territory_assets",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_assets_TerritoryId_Status",
                table: "territory_assets",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_assets_TerritoryId_Type",
                table: "territory_assets",
                columns: new[] { "TerritoryId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_characterizations_TerritoryId",
                table: "territory_characterizations",
                column: "TerritoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_territory_events_Latitude_Longitude",
                table: "territory_events",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_events_TerritoryId_StartsAtUtc",
                table: "territory_events",
                columns: new[] { "TerritoryId", "StartsAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_events_TerritoryId_Status",
                table: "territory_events",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_join_request_recipients_JoinRequestId",
                table: "territory_join_request_recipients",
                column: "JoinRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_join_request_recipients_RecipientUserId",
                table: "territory_join_request_recipients",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_join_requests_RequesterUserId",
                table: "territory_join_requests",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_join_requests_TerritoryId",
                table: "territory_join_requests",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_join_requests_TerritoryId_RequesterUserId",
                table: "territory_join_requests",
                columns: new[] { "TerritoryId", "RequesterUserId" },
                filter: "\"Status\" = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_territory_memberships_TerritoryId",
                table: "territory_memberships",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_memberships_UserId",
                table: "territory_memberships",
                column: "UserId",
                unique: true,
                filter: "\"Role\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_territory_memberships_UserId_TerritoryId",
                table: "territory_memberships",
                columns: new[] { "UserId", "TerritoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_territory_moderation_rules_CreatedByVotingId",
                table: "territory_moderation_rules",
                column: "CreatedByVotingId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_moderation_rules_TerritoryId",
                table: "territory_moderation_rules",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_moderation_rules_TerritoryId_IsActive",
                table: "territory_moderation_rules",
                columns: new[] { "TerritoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_payout_configs_TerritoryId",
                table: "territory_payout_configs",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_payout_configs_TerritoryId_IsActive",
                table: "territory_payout_configs",
                columns: new[] { "TerritoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_territory_stores_OwnerUserId",
                table: "territory_stores",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_stores_TerritoryId",
                table: "territory_stores",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_stores_TerritoryId_OwnerUserId",
                table: "territory_stores",
                columns: new[] { "TerritoryId", "OwnerUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_histories_ChangedByUserId",
                table: "transaction_status_histories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_histories_FinancialTransactionId",
                table: "transaction_status_histories",
                column: "FinancialTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_BlockerUserId",
                table: "user_blocks",
                column: "BlockerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_RequesterUserId",
                table: "user_connections",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_RequesterUserId_TargetUserId",
                table: "user_connections",
                columns: new[] { "RequesterUserId", "TargetUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_Status",
                table: "user_connections",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_user_connections_TargetUserId",
                table: "user_connections",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_DeviceToken",
                table: "user_devices",
                column: "DeviceToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_UserId",
                table: "user_devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_UserId_IsActive",
                table: "user_devices",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_InterestTag",
                table: "user_interests",
                column: "InterestTag");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_UserId",
                table: "user_interests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_UserId_InterestTag",
                table: "user_interests",
                columns: new[] { "UserId", "InterestTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_CreatedAtUtc",
                table: "user_notifications",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_SourceOutboxId_UserId",
                table: "user_notifications",
                columns: new[] { "SourceOutboxId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_UserId",
                table: "user_notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_UserId",
                table: "user_preferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_AuthProvider_ExternalId",
                table: "users",
                columns: new[] { "AuthProvider", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votes_UserId",
                table: "votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_votes_VotingId",
                table: "votes",
                column: "VotingId");

            migrationBuilder.CreateIndex(
                name: "IX_votes_VotingId_UserId",
                table: "votes",
                columns: new[] { "VotingId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votings_CreatedByUserId",
                table: "votings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_votings_Status",
                table: "votings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_votings_TerritoryId",
                table: "votings",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_votings_TerritoryId_Status",
                table: "votings",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_work_items_Status",
                table: "work_items",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_work_items_SubjectType_SubjectId",
                table: "work_items",
                columns: new[] { "SubjectType", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_work_items_TerritoryId_Status",
                table: "work_items",
                columns: new[] { "TerritoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_work_items_Type",
                table: "work_items",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "active_territories");

            migrationBuilder.DropTable(
                name: "asset_geo_anchors");

            migrationBuilder.DropTable(
                name: "asset_validations");

            migrationBuilder.DropTable(
                name: "audit_entries");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.DropTable(
                name: "chat_conversation_participants");

            migrationBuilder.DropTable(
                name: "chat_conversation_stats");

            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "checkout_items");

            migrationBuilder.DropTable(
                name: "checkouts");

            migrationBuilder.DropTable(
                name: "community_posts");

            migrationBuilder.DropTable(
                name: "connection_privacy_settings");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "document_evidences");

            migrationBuilder.DropTable(
                name: "email_queue_items");

            migrationBuilder.DropTable(
                name: "event_participations");

            migrationBuilder.DropTable(
                name: "feature_flags");

            migrationBuilder.DropTable(
                name: "health_alerts");

            migrationBuilder.DropTable(
                name: "item_inquiries");

            migrationBuilder.DropTable(
                name: "map_entities");

            migrationBuilder.DropTable(
                name: "map_entity_relations");

            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.DropTable(
                name: "media_attachments");

            migrationBuilder.DropTable(
                name: "media_storage_configs");

            migrationBuilder.DropTable(
                name: "membership_capabilities");

            migrationBuilder.DropTable(
                name: "membership_settings");

            migrationBuilder.DropTable(
                name: "moderation_reports");

            migrationBuilder.DropTable(
                name: "notification_configs");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "platform_expense_transactions");

            migrationBuilder.DropTable(
                name: "platform_fee_configs");

            migrationBuilder.DropTable(
                name: "platform_financial_balances");

            migrationBuilder.DropTable(
                name: "platform_revenue_transactions");

            migrationBuilder.DropTable(
                name: "post_comments");

            migrationBuilder.DropTable(
                name: "post_likes");

            migrationBuilder.DropTable(
                name: "post_shares");

            migrationBuilder.DropTable(
                name: "privacy_policies");

            migrationBuilder.DropTable(
                name: "privacy_policy_acceptances");

            migrationBuilder.DropTable(
                name: "reconciliation_records");

            migrationBuilder.DropTable(
                name: "sanctions");

            migrationBuilder.DropTable(
                name: "seller_balances");

            migrationBuilder.DropTable(
                name: "seller_transactions");

            migrationBuilder.DropTable(
                name: "store_item_ratings");

            migrationBuilder.DropTable(
                name: "store_items");

            migrationBuilder.DropTable(
                name: "store_rating_responses");

            migrationBuilder.DropTable(
                name: "store_ratings");

            migrationBuilder.DropTable(
                name: "subscription_coupons");

            migrationBuilder.DropTable(
                name: "subscription_payments");

            migrationBuilder.DropTable(
                name: "subscription_plan_histories");

            migrationBuilder.DropTable(
                name: "subscription_plans");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "system_configs");

            migrationBuilder.DropTable(
                name: "system_permissions");

            migrationBuilder.DropTable(
                name: "terms_acceptances");

            migrationBuilder.DropTable(
                name: "terms_of_services");

            migrationBuilder.DropTable(
                name: "territories");

            migrationBuilder.DropTable(
                name: "territory_assets");

            migrationBuilder.DropTable(
                name: "territory_characterizations");

            migrationBuilder.DropTable(
                name: "territory_events");

            migrationBuilder.DropTable(
                name: "territory_join_request_recipients");

            migrationBuilder.DropTable(
                name: "territory_join_requests");

            migrationBuilder.DropTable(
                name: "territory_media_configs");

            migrationBuilder.DropTable(
                name: "territory_moderation_rules");

            migrationBuilder.DropTable(
                name: "territory_payout_configs");

            migrationBuilder.DropTable(
                name: "territory_stores");

            migrationBuilder.DropTable(
                name: "transaction_status_histories");

            migrationBuilder.DropTable(
                name: "user_blocks");

            migrationBuilder.DropTable(
                name: "user_connections");

            migrationBuilder.DropTable(
                name: "user_devices");

            migrationBuilder.DropTable(
                name: "user_interests");

            migrationBuilder.DropTable(
                name: "user_media_preferences");

            migrationBuilder.DropTable(
                name: "user_notifications");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "votes");

            migrationBuilder.DropTable(
                name: "work_items");

            migrationBuilder.DropTable(
                name: "chat_conversations");

            migrationBuilder.DropTable(
                name: "territory_memberships");

            migrationBuilder.DropTable(
                name: "financial_transactions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "votings");
        }
    }
}
