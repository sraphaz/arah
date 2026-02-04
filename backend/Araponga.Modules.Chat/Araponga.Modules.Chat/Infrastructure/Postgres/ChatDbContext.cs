using Araponga.Modules.Chat.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Chat.Infrastructure.Postgres;

public sealed class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatConversationRecord> ChatConversations => Set<ChatConversationRecord>();
    public DbSet<ChatConversationParticipantRecord> ChatConversationParticipants => Set<ChatConversationParticipantRecord>();
    public DbSet<ChatMessageRecord> ChatMessages => Set<ChatMessageRecord>();
    public DbSet<ChatConversationStatsRecord> ChatConversationStats => Set<ChatConversationStatsRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatConversationRecord>(entity =>
        {
            entity.ToTable("chat_conversations");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Kind).HasConversion<int>().IsRequired();
            entity.Property(c => c.Status).HasConversion<int>().IsRequired();
            entity.Property(c => c.Name).HasMaxLength(120);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.ApprovedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.LockedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.DisabledAtUtc).HasColumnType("timestamp with time zone");

            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.Kind, c.Status, c.CreatedAtUtc });

            entity.HasIndex(c => new { c.TerritoryId, c.Kind })
                .IsUnique()
                .HasFilter("\"TerritoryId\" IS NOT NULL AND \"Kind\" IN (1, 2)");
        });

        modelBuilder.Entity<ChatConversationParticipantRecord>(entity =>
        {
            entity.ToTable("chat_conversation_participants");
            entity.HasKey(p => new { p.ConversationId, p.UserId });
            entity.Property(p => p.Role).HasConversion<int>().IsRequired();
            entity.Property(p => p.JoinedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.LeftAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.MutedUntilUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.LastReadAtUtc).HasColumnType("timestamp with time zone");

            entity.HasOne<ChatConversationRecord>()
                .WithMany()
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.UserId);
        });

        modelBuilder.Entity<ChatMessageRecord>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.ContentType).HasConversion<int>().IsRequired();
            entity.Property(m => m.Text).HasColumnType("text");
            entity.Property(m => m.PayloadJson).HasColumnType("text");
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.EditedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.DeletedAtUtc).HasColumnType("timestamp with time zone");

            entity.HasOne<ChatConversationRecord>()
                .WithMany()
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => m.ConversationId);
            entity.HasIndex(m => new { m.ConversationId, m.CreatedAtUtc, m.Id });
            entity.HasIndex(m => new { m.SenderUserId, m.CreatedAtUtc });
        });

        modelBuilder.Entity<ChatConversationStatsRecord>(entity =>
        {
            entity.ToTable("chat_conversation_stats");
            entity.HasKey(s => s.ConversationId);
            entity.Property(s => s.LastPreview).HasMaxLength(200);
            entity.Property(s => s.LastMessageAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.MessageCount).IsRequired();

            entity.HasOne<ChatConversationRecord>()
                .WithOne()
                .HasForeignKey<ChatConversationStatsRecord>(s => s.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.LastMessageAtUtc);
        });
    }
}
