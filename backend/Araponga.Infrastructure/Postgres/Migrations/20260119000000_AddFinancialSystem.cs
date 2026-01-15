using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddFinancialSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Financial Transactions
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
                table.ForeignKey(
                    name: "FK_financial_transactions_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_financial_transactions_TerritoryId",
            table: "financial_transactions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_financial_transactions_TerritoryId_Type",
            table: "financial_transactions",
            columns: new[] { "TerritoryId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_financial_transactions_TerritoryId_Status",
            table: "financial_transactions",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_financial_transactions_RelatedEntityId",
            table: "financial_transactions",
            column: "RelatedEntityId");

        // Transaction Status History
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
                    name: "FK_transaction_status_histories_financial_transactions_FinancialTransactionId",
                    column: x => x.FinancialTransactionId,
                    principalTable: "financial_transactions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_transaction_status_histories_FinancialTransactionId",
            table: "transaction_status_histories",
            column: "FinancialTransactionId");

        migrationBuilder.CreateIndex(
            name: "IX_transaction_status_histories_ChangedByUserId",
            table: "transaction_status_histories",
            column: "ChangedByUserId");

        // Seller Balances
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
                table.ForeignKey(
                    name: "FK_seller_balances_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_seller_balances_users_SellerUserId",
                    column: x => x.SellerUserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_seller_balances_TerritoryId_SellerUserId",
            table: "seller_balances",
            columns: new[] { "TerritoryId", "SellerUserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_seller_balances_TerritoryId",
            table: "seller_balances",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_seller_balances_SellerUserId",
            table: "seller_balances",
            column: "SellerUserId");

        // Seller Transactions
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
                table.ForeignKey(
                    name: "FK_seller_transactions_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_seller_transactions_territory_stores_StoreId",
                    column: x => x.StoreId,
                    principalTable: "territory_stores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_seller_transactions_checkouts_CheckoutId",
                    column: x => x.CheckoutId,
                    principalTable: "checkouts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_seller_transactions_users_SellerUserId",
                    column: x => x.SellerUserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_TerritoryId",
            table: "seller_transactions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_CheckoutId",
            table: "seller_transactions",
            column: "CheckoutId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_SellerUserId",
            table: "seller_transactions",
            column: "SellerUserId");

        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_TerritoryId_Status",
            table: "seller_transactions",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_PayoutId",
            table: "seller_transactions",
            column: "PayoutId");

        // Platform Financial Balances
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
                table.ForeignKey(
                    name: "FK_platform_financial_balances_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_platform_financial_balances_TerritoryId",
            table: "platform_financial_balances",
            column: "TerritoryId",
            unique: true);

        // Platform Revenue Transactions
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
                table.ForeignKey(
                    name: "FK_platform_revenue_transactions_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_platform_revenue_transactions_checkouts_CheckoutId",
                    column: x => x.CheckoutId,
                    principalTable: "checkouts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_platform_revenue_transactions_TerritoryId",
            table: "platform_revenue_transactions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_platform_revenue_transactions_CheckoutId",
            table: "platform_revenue_transactions",
            column: "CheckoutId");

        // Platform Expense Transactions
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
                table.ForeignKey(
                    name: "FK_platform_expense_transactions_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_platform_expense_transactions_seller_transactions_SellerTransactionId",
                    column: x => x.SellerTransactionId,
                    principalTable: "seller_transactions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_platform_expense_transactions_TerritoryId",
            table: "platform_expense_transactions",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_platform_expense_transactions_SellerTransactionId",
            table: "platform_expense_transactions",
            column: "SellerTransactionId");

        migrationBuilder.CreateIndex(
            name: "IX_platform_expense_transactions_PayoutId",
            table: "platform_expense_transactions",
            column: "PayoutId");

        // Reconciliation Records
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
                table.ForeignKey(
                    name: "FK_reconciliation_records_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_reconciliation_records_TerritoryId",
            table: "reconciliation_records",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_reconciliation_records_TerritoryId_Status",
            table: "reconciliation_records",
            columns: new[] { "TerritoryId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_reconciliation_records_ReconciliationDate",
            table: "reconciliation_records",
            column: "ReconciliationDate");

        // Territory Payout Config
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
                table.ForeignKey(
                    name: "FK_territory_payout_configs_territories_TerritoryId",
                    column: x => x.TerritoryId,
                    principalTable: "territories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_territory_payout_configs_TerritoryId",
            table: "territory_payout_configs",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_payout_configs_TerritoryId_IsActive",
            table: "territory_payout_configs",
            columns: new[] { "TerritoryId", "IsActive" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "reconciliation_records");
        migrationBuilder.DropTable(name: "platform_expense_transactions");
        migrationBuilder.DropTable(name: "platform_revenue_transactions");
        migrationBuilder.DropTable(name: "platform_financial_balances");
        migrationBuilder.DropTable(name: "transaction_status_histories");
        migrationBuilder.DropTable(name: "seller_transactions");
        migrationBuilder.DropTable(name: "seller_balances");
        migrationBuilder.DropTable(name: "financial_transactions");
        migrationBuilder.DropTable(name: "territory_payout_configs");
    }
}
