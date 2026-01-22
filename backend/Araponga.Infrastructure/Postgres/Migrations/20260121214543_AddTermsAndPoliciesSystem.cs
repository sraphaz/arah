using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddTermsAndPoliciesSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditCount",
                table: "community_posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAtUtc",
                table: "community_posts",
                type: "timestamp with time zone",
                nullable: true);

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
                name: "IX_territory_payout_configs_TerritoryId",
                table: "territory_payout_configs",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_territory_payout_configs_TerritoryId_IsActive",
                table: "territory_payout_configs",
                columns: new[] { "TerritoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_histories_ChangedByUserId",
                table: "transaction_status_histories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_status_histories_FinancialTransactionId",
                table: "transaction_status_histories",
                column: "FinancialTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.DropTable(
                name: "media_attachments");

            migrationBuilder.DropTable(
                name: "platform_expense_transactions");

            migrationBuilder.DropTable(
                name: "platform_financial_balances");

            migrationBuilder.DropTable(
                name: "platform_revenue_transactions");

            migrationBuilder.DropTable(
                name: "privacy_policies");

            migrationBuilder.DropTable(
                name: "privacy_policy_acceptances");

            migrationBuilder.DropTable(
                name: "reconciliation_records");

            migrationBuilder.DropTable(
                name: "seller_balances");

            migrationBuilder.DropTable(
                name: "seller_transactions");

            migrationBuilder.DropTable(
                name: "store_item_ratings");

            migrationBuilder.DropTable(
                name: "store_rating_responses");

            migrationBuilder.DropTable(
                name: "store_ratings");

            migrationBuilder.DropTable(
                name: "terms_acceptances");

            migrationBuilder.DropTable(
                name: "terms_of_services");

            migrationBuilder.DropTable(
                name: "territory_payout_configs");

            migrationBuilder.DropTable(
                name: "transaction_status_histories");

            migrationBuilder.DropTable(
                name: "financial_transactions");

            migrationBuilder.DropColumn(
                name: "EditCount",
                table: "community_posts");

            migrationBuilder.DropColumn(
                name: "EditedAtUtc",
                table: "community_posts");
        }
    }
}
