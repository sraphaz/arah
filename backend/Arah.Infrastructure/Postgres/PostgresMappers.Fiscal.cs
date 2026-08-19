using Arah.Domain.Fiscal;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static TerritoryFiscalPackBindingRecord ToRecord(this TerritoryFiscalPackBinding binding) =>
        new()
        {
            Id = binding.Id,
            TerritoryId = binding.TerritoryId,
            PackId = binding.PackId,
            Status = (int)binding.Status,
            ActivatedByUserId = binding.ActivatedByUserId,
            ActivatedAtUtc = binding.ActivatedAtUtc,
            MunicipalityIbge = binding.MunicipalityIbge,
            UpdatedAtUtc = binding.UpdatedAtUtc
        };

    public static TerritoryFiscalPackBinding ToDomain(this TerritoryFiscalPackBindingRecord record) =>
        new(
            record.Id,
            record.TerritoryId,
            record.PackId,
            (FiscalPackBindingStatus)record.Status,
            record.ActivatedByUserId,
            record.ActivatedAtUtc,
            record.MunicipalityIbge,
            record.UpdatedAtUtc);

    public static TerritoryPaymentMethodsConfigRecord ToRecord(this TerritoryPaymentMethodsConfig config) =>
        new()
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            MethodsCsv = string.Join(',', config.Methods.Select(m => m.ToString())),
            PspProvider = config.PspProvider,
            UpdatedAtUtc = config.UpdatedAtUtc
        };

    public static TerritoryPaymentMethodsConfig ToDomain(this TerritoryPaymentMethodsConfigRecord record)
    {
        var methods = string.IsNullOrWhiteSpace(record.MethodsCsv)
            ? Array.Empty<TerritoryPaymentMethodKind>()
            : record.MethodsCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Enum.Parse<TerritoryPaymentMethodKind>)
                .ToArray();

        return new TerritoryPaymentMethodsConfig(
            record.Id,
            record.TerritoryId,
            methods,
            record.PspProvider,
            record.UpdatedAtUtc);
    }
}
