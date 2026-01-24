using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário pelo provedor de autenticação e ID externo.
    /// </summary>
    /// <param name="authProvider">Provedor de autenticação (ex: "google", "apple").</param>
    /// <param name="externalId">ID único do usuário no provedor.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByAuthProviderAsync(string authProvider, string externalId, CancellationToken cancellationToken);

    /// <summary>
    /// Busca um usuário pelo ID único.
    /// </summary>
    /// <param name="id">Identificador único do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona um novo usuário ao repositório.
    /// </summary>
    /// <param name="user">Usuário a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task AddAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um usuário existente no repositório.
    /// </summary>
    /// <param name="user">Usuário com dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Busca múltiplos usuários pelos seus IDs em uma única operação (batch).
    /// </summary>
    /// <param name="userIds">Coleção de IDs dos usuários a buscar.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de usuários encontrados.</returns>
    Task<IReadOnlyList<User>> ListByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Busca um usuário pelo email.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Usuário encontrado ou null se não existir.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
