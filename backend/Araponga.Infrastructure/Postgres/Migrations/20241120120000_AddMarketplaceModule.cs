using Microsoft.EntityFrameworkCore.Migrations;

namespace Araponga.Infrastructure.Postgres.Migrations;

public partial class AddMarketplaceModule : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
            name: "store_listings",
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
                table.PrimaryKey("PK_store_listings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "listing_inquiries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                BatchId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_listing_inquiries", x => x.Id);
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
            name: "cart_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CartId = table.Column<Guid>(type: "uuid", nullable: false),
                ListingId = table.Column<Guid>(type: "uuid", nullable: false),
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
            name: "checkout_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CheckoutId = table.Column<Guid>(type: "uuid", nullable: false),
                ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                ListingType = table.Column<int>(type: "integer", nullable: false),
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
            name: "platform_fee_configs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TerritoryId = table.Column<Guid>(type: "uuid", nullable: false),
                ListingType = table.Column<int>(type: "integer", nullable: false),
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

        migrationBuilder.CreateIndex(
            name: "IX_territory_stores_TerritoryId",
            table: "territory_stores",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_stores_OwnerUserId",
            table: "territory_stores",
            column: "OwnerUserId");

        migrationBuilder.CreateIndex(
            name: "IX_territory_stores_TerritoryId_OwnerUserId",
            table: "territory_stores",
            columns: new[] { "TerritoryId", "OwnerUserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_store_listings_TerritoryId",
            table: "store_listings",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_store_listings_StoreId",
            table: "store_listings",
            column: "StoreId");

        migrationBuilder.CreateIndex(
            name: "IX_store_listings_TerritoryId_Type_Status",
            table: "store_listings",
            columns: new[] { "TerritoryId", "Type", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_listing_inquiries_TerritoryId",
            table: "listing_inquiries",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_listing_inquiries_ListingId",
            table: "listing_inquiries",
            column: "ListingId");

        migrationBuilder.CreateIndex(
            name: "IX_listing_inquiries_StoreId",
            table: "listing_inquiries",
            column: "StoreId");

        migrationBuilder.CreateIndex(
            name: "IX_listing_inquiries_FromUserId",
            table: "listing_inquiries",
            column: "FromUserId");

        migrationBuilder.CreateIndex(
            name: "IX_carts_TerritoryId",
            table: "carts",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_carts_UserId",
            table: "carts",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_carts_TerritoryId_UserId",
            table: "carts",
            columns: new[] { "TerritoryId", "UserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_cart_items_CartId",
            table: "cart_items",
            column: "CartId");

        migrationBuilder.CreateIndex(
            name: "IX_cart_items_ListingId",
            table: "cart_items",
            column: "ListingId");

        migrationBuilder.CreateIndex(
            name: "IX_cart_items_CartId_ListingId",
            table: "cart_items",
            columns: new[] { "CartId", "ListingId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_checkouts_TerritoryId",
            table: "checkouts",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_checkouts_BuyerUserId",
            table: "checkouts",
            column: "BuyerUserId");

        migrationBuilder.CreateIndex(
            name: "IX_checkouts_StoreId",
            table: "checkouts",
            column: "StoreId");

        migrationBuilder.CreateIndex(
            name: "IX_checkout_items_CheckoutId",
            table: "checkout_items",
            column: "CheckoutId");

        migrationBuilder.CreateIndex(
            name: "IX_checkout_items_ListingId",
            table: "checkout_items",
            column: "ListingId");

        migrationBuilder.CreateIndex(
            name: "IX_platform_fee_configs_TerritoryId",
            table: "platform_fee_configs",
            column: "TerritoryId");

        migrationBuilder.CreateIndex(
            name: "IX_platform_fee_configs_TerritoryId_ListingType_IsActive",
            table: "platform_fee_configs",
            columns: new[] { "TerritoryId", "ListingType", "IsActive" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "platform_fee_configs");
        migrationBuilder.DropTable(name: "checkout_items");
        migrationBuilder.DropTable(name: "checkouts");
        migrationBuilder.DropTable(name: "cart_items");
        migrationBuilder.DropTable(name: "carts");
        migrationBuilder.DropTable(name: "listing_inquiries");
        migrationBuilder.DropTable(name: "store_listings");
        migrationBuilder.DropTable(name: "territory_stores");
    }
}
