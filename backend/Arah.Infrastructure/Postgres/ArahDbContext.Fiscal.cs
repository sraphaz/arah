using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureFiscal(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryFiscalPackBindingRecord>(entity =>
        {
            entity.ToTable("territory_fiscal_pack_bindings");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.PackId).HasMaxLength(64).IsRequired();
            entity.Property(b => b.Status).IsRequired();
            entity.Property(b => b.MunicipalityIbge).HasMaxLength(7);
            entity.Property(b => b.ActivatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.TerritoryId).IsUnique();
        });

        modelBuilder.Entity<TerritoryPaymentMethodsConfigRecord>(entity =>
        {
            entity.ToTable("territory_payment_methods_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.MethodsCsv).HasMaxLength(128).IsRequired();
            entity.Property(c => c.PspProvider).HasMaxLength(64);
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId).IsUnique();
        });
    }
}
