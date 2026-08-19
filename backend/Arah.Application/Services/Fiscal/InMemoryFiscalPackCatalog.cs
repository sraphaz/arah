using Arah.Application.Interfaces;
using Arah.Domain.Fiscal;

namespace Arah.Application.Services.Fiscal;

/// <summary>
/// Catálogo estático MVP — Brazil.v1 único; novos packs = novas entradas sem mutar Territory.
/// </summary>
public sealed class InMemoryFiscalPackCatalog : IFiscalPackCatalog
{
    private static readonly IReadOnlyList<FiscalPackDefinition> Packs =
        new[] { FiscalPackDefinition.BrazilV1 };

    public IReadOnlyList<FiscalPackDefinition> ListPacks() => Packs;

    public FiscalPackDefinition? GetById(string packId)
    {
        if (string.IsNullOrWhiteSpace(packId))
        {
            return null;
        }

        var normalized = packId.Trim().ToLowerInvariant();
        return Packs.FirstOrDefault(p => p.Id == normalized);
    }
}
