using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddPerformanceIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Índices compostos para queries comuns de feed
        migrationBuilder.CreateIndex(
            name: "IX_community_posts_territory_status_created",
            table: "community_posts",
            columns: new[] { "territory_id", "status", "created_at_utc" },
            filter: "\"status\" = 0"); // PostStatus.Published = 0

        // Índice para queries de eventos por data
        migrationBuilder.CreateIndex(
            name: "IX_territory_events_territory_starts_status",
            table: "territory_events",
            columns: new[] { "territory_id", "starts_at_utc", "status" });

        // Índice para queries de participações de eventos
        migrationBuilder.CreateIndex(
            name: "IX_event_participations_user_event",
            table: "event_participations",
            columns: new[] { "user_id", "event_id" });

        // Índice para queries de notificações não lidas
        migrationBuilder.CreateIndex(
            name: "IX_user_notifications_user_read_created",
            table: "user_notifications",
            columns: new[] { "user_id", "read_at_utc", "created_at_utc" },
            filter: "\"read_at_utc\" IS NULL");

        // Índice para queries de marketplace por status
        migrationBuilder.CreateIndex(
            name: "IX_territory_stores_territory_status",
            table: "territory_stores",
            columns: new[] { "territory_id", "status" });

        // Índice para queries de items por store e status
        migrationBuilder.CreateIndex(
            name: "IX_store_items_store_status_created",
            table: "store_items",
            columns: new[] { "store_id", "status", "created_at_utc" });

        // Índice para queries de checkouts por status e data
        migrationBuilder.CreateIndex(
            name: "IX_checkouts_territory_status_created",
            table: "checkouts",
            columns: new[] { "territory_id", "status", "created_at_utc" });

        // Índice para queries de transações de seller
        migrationBuilder.CreateIndex(
            name: "IX_seller_transactions_seller_status_paid",
            table: "seller_transactions",
            columns: new[] { "seller_user_id", "status", "paid_at_utc" });

        // Índice para queries de analytics (aceites de termos)
        migrationBuilder.CreateIndex(
            name: "IX_terms_acceptances_user_accepted",
            table: "terms_acceptances",
            columns: new[] { "user_id", "accepted_at_utc" });

        // Índice para queries de analytics (aceites de privacidade)
        migrationBuilder.CreateIndex(
            name: "IX_privacy_policy_acceptances_user_accepted",
            table: "privacy_policy_acceptances",
            columns: new[] { "user_id", "accepted_at_utc" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_community_posts_territory_status_created",
            table: "community_posts");

        migrationBuilder.DropIndex(
            name: "IX_territory_events_territory_starts_status",
            table: "territory_events");

        migrationBuilder.DropIndex(
            name: "IX_event_participations_user_event",
            table: "event_participations");

        migrationBuilder.DropIndex(
            name: "IX_user_notifications_user_read_created",
            table: "user_notifications");

        migrationBuilder.DropIndex(
            name: "IX_territory_stores_territory_status",
            table: "territory_stores");

        migrationBuilder.DropIndex(
            name: "IX_store_items_store_status_created",
            table: "store_items");

        migrationBuilder.DropIndex(
            name: "IX_checkouts_territory_status_created",
            table: "checkouts");

        migrationBuilder.DropIndex(
            name: "IX_seller_transactions_seller_status_paid",
            table: "seller_transactions");

        migrationBuilder.DropIndex(
            name: "IX_terms_acceptances_user_accepted",
            table: "terms_acceptances");

        migrationBuilder.DropIndex(
            name: "IX_privacy_policy_acceptances_user_accepted",
            table: "privacy_policy_acceptances");
    }
}
