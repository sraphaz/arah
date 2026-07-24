using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureOutbox(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessageRecord>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).HasMaxLength(200).IsRequired();
            entity.Property(o => o.PayloadJson).HasColumnType("jsonb");
            entity.Property(o => o.OccurredAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessAfterUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.Attempts).HasDefaultValue(0);
            entity.Property(o => o.LastError).HasColumnType("text");
            entity.HasIndex(o => new { o.ProcessedAtUtc, o.ProcessAfterUtc });
            entity.HasIndex(o => new { o.Type, o.ProcessedAtUtc });
        });
    }
}
