namespace Araponga.Modules.Moderation.Domain.Work;

/// <summary>
/// Resultado final do item de trabalho quando concluído por humano ou automação.
/// </summary>
public enum WorkItemOutcome
{
    None = 0,
    Approved = 1,
    Rejected = 2,
    NoAction = 3
}
