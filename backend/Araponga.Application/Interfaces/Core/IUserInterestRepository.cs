using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar interesses de usuários.
/// </summary>
public interface IUserInterestRepository
{
    /// <summary>
    /// Adiciona um novo interesse para um usuário.
    /// </summary>
    Task AddAsync(UserInterest interest, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um interesse de um usuário.
    /// </summary>
    Task RemoveAsync(Guid userId, string interestTag, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os interesses de um usuário.
    /// </summary>
    Task<IReadOnlyList<UserInterest>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os usuários que têm um interesse específico em um território.
    /// </summary>
    Task<IReadOnlyList<Guid>> ListUserIdsByInterestAsync(string interestTag, Guid territoryId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se um usuário já possui um interesse específico.
    /// </summary>
    Task<bool> ExistsAsync(Guid userId, string interestTag, CancellationToken cancellationToken);

    /// <summary>
    /// Conta quantos interesses um usuário possui.
    /// </summary>
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
