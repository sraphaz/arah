namespace Araponga.Modules.Moderation.Domain.Work;

/// <summary>
/// Tipos de itens de trabalho (fila genérica) para fallback humano.
/// </summary>
public enum WorkItemType
{
    IdentityVerification = 1,
    ResidencyVerification = 2,
    AssetCuration = 3,
    ModerationCase = 4,
    Other = 99
}
