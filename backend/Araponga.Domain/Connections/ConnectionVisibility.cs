namespace Araponga.Domain.Connections;

/// <summary>
/// Quem pode ver a lista de conexões do usuário.
/// </summary>
public enum ConnectionVisibility
{
    /// <summary>Apenas eu.</summary>
    OnlyMe = 0,

    /// <summary>Minhas conexões.</summary>
    MyConnections = 1,

    /// <summary>Todos no território.</summary>
    TerritoryMembers = 2,

    /// <summary>Todos.</summary>
    Everyone = 3
}
