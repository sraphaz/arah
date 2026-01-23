namespace Araponga.Domain.Governance;

/// <summary>
/// Status de uma votação.
/// </summary>
public enum VotingStatus
{
    /// <summary>
    /// Rascunho (ainda não aberta para votação).
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Aberta (aceitando votos).
    /// </summary>
    Open = 1,

    /// <summary>
    /// Fechada (não aceita mais votos, aguardando processamento).
    /// </summary>
    Closed = 2,

    /// <summary>
    /// Aprovada (resultado aplicado).
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Rejeitada (resultado não aplicado).
    /// </summary>
    Rejected = 4
}
