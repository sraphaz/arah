using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddSubscriptionsSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Subscription Plans
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
                table.ForeignKey(
                    name: "FK_subscription_plans_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_subscription_plans_users_CreatedByUserId",
                    column: x => x.CreatedByUserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plans_Scope",
            table: "subscription_plans",
            column: "Scope");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plans_TerritoryId",
            table: "subscription_plans",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plans_Scope_TerritoryId",
            table: "subscription_plans",
            columns: new[] { "Scope", "TerritoryId" });

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plans_Scope_IsDefault",
            table: "subscription_plans",
            columns: new[] { "Scope", "IsDefault" });

        // Subscriptions
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
                table.ForeignKey(
                    name: "FK_subscriptions_users_UserId",
                    column: x => x.UserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_subscriptions_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_subscriptions_subscription_plans_PlanId",
                    column: x => x.PlanId,
                    principalTable: "subscription_plans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_subscriptions_UserId",
            table: "subscriptions",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_subscriptions_TerritoryId",
            table: "subscriptions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_subscriptions_PlanId",
            table: "subscriptions",
            column: "PlanId");

        migrationBuilder.CreateIndex(
            name: "IX_subscriptions_UserId_TerritoryId",
            table: "subscriptions",
            columns: new[] { "UserId", "TerritoryId" });

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

        // Subscription Payments
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
                table.ForeignKey(
                    name: "FK_subscription_payments_subscriptions_SubscriptionId",
                    column: x => x.SubscriptionId,
                    principalTable: "subscriptions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_subscription_payments_SubscriptionId",
            table: "subscription_payments",
            column: "SubscriptionId");

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

        // Coupons
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

        // Subscription Coupons
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
                table.ForeignKey(
                    name: "FK_subscription_coupons_subscriptions_SubscriptionId",
                    column: x => x.SubscriptionId,
                    principalTable: "subscriptions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_subscription_coupons_coupons_CouponId",
                    column: x => x.CouponId,
                    principalTable: "coupons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_subscription_coupons_SubscriptionId",
            table: "subscription_coupons",
            column: "SubscriptionId");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_coupons_CouponId",
            table: "subscription_coupons",
            column: "CouponId");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_coupons_SubscriptionId_CouponId",
            table: "subscription_coupons",
            columns: new[] { "SubscriptionId", "CouponId" },
            unique: true);

        // Subscription Plan History
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
                table.ForeignKey(
                    name: "FK_subscription_plan_histories_subscription_plans_PlanId",
                    column: x => x.PlanId,
                    principalTable: "subscription_plans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_subscription_plan_histories_users_ChangedByUserId",
                    column: x => x.ChangedByUserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plan_histories_PlanId",
            table: "subscription_plan_histories",
            column: "PlanId");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plan_histories_ChangedByUserId",
            table: "subscription_plan_histories",
            column: "ChangedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_subscription_plan_histories_PlanId_ChangedAtUtc",
            table: "subscription_plan_histories",
            columns: new[] { "PlanId", "ChangedAtUtc" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "subscription_plan_histories");
        migrationBuilder.DropTable(name: "subscription_coupons");
        migrationBuilder.DropTable(name: "subscription_payments");
        migrationBuilder.DropTable(name: "coupons");
        migrationBuilder.DropTable(name: "subscriptions");
        migrationBuilder.DropTable(name: "subscription_plans");
    }
}
