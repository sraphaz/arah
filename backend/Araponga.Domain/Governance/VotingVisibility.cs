namespace Araponga.Domain.Governance;

/// <summary>
/// Visibilidade de uma votação (quem pode votar).
/// </summary>
public enum VotingVisibility
{
    /// <summary>
    /// Todos os membros do território podem votar.
    /// </summary>
    AllMembers = 0,

    /// <summary>
    /// Apenas residents podem votar.
    /// </summary>
    ResidentsOnly = 1,

    /// <summary>
    /// Apenas curadores/moderadores podem votar.
    /// </summary>
    CuratorsOnly = 2
}
