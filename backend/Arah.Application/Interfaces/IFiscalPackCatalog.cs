using Arah.Domain.Fiscal;

namespace Arah.Application.Interfaces;

/// <summary>
/// Catálogo de pacotes fiscais disponíveis na instância.
/// </summary>
public interface IFiscalPackCatalog
{
    IReadOnlyList<FiscalPackDefinition> ListPacks();
    FiscalPackDefinition? GetById(string packId);
}
