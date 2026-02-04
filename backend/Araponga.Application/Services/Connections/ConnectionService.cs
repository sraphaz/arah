using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Domain.Connections;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Application.Services.Connections;

/// <summary>
/// Serviço de aplicação para gerenciamento de conexões (círculo de amigos).
/// </summary>
public sealed class ConnectionService
{
    private const int DefaultSearchLimit = 20;
    private const int DefaultSuggestionsLimit = 10;

    private readonly IUserConnectionRepository _connectionRepository;
    private readonly IConnectionPrivacySettingsRepository _privacyRepository;
    private readonly IUserBlockRepository _userBlockRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public ConnectionService(
        IUserConnectionRepository connectionRepository,
        IConnectionPrivacySettingsRepository privacyRepository,
        IUserBlockRepository userBlockRepository,
        IUserRepository userRepository,
        ITerritoryMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork,
        IEventBus eventBus)
    {
        _connectionRepository = connectionRepository;
        _privacyRepository = privacyRepository;
        _userBlockRepository = userBlockRepository;
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<Result<UserConnection>> RequestConnectionAsync(
        Guid requesterUserId,
        Guid targetUserId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        if (requesterUserId == targetUserId)
            return Result<UserConnection>.Failure("Não é possível enviar solicitação para si mesmo.");

        var exists = await _connectionRepository.ExistsAsync(requesterUserId, targetUserId, cancellationToken);
        if (exists)
            return Result<UserConnection>.Failure("Já existe uma conexão ou solicitação entre os usuários.");

        var blocked = await _userBlockRepository.GetBlockedUserIdsAsync(targetUserId, cancellationToken);
        if (blocked.Contains(requesterUserId))
            return Result<UserConnection>.Failure("Não é possível enviar solicitação para este usuário.");

        var targetPrivacy = await _privacyRepository.GetByUserIdAsync(targetUserId, cancellationToken);
        var settings = targetPrivacy ?? ConnectionPrivacySettings.CreateDefault(targetUserId, DateTime.UtcNow);
        if (settings.WhoCanAddMe == ConnectionRequestPolicy.Disabled)
            return Result<UserConnection>.Failure("Este usuário não aceita novas conexões.");

        if (settings.WhoCanAddMe == ConnectionRequestPolicy.ResidentsOnly)
        {
            var requesterMemberships = await _membershipRepository.ListByUserAsync(requesterUserId, cancellationToken);
            var targetMemberships = await _membershipRepository.ListByUserAsync(targetUserId, cancellationToken);
            var requesterTerritoryIds = requesterMemberships.Select(m => m.TerritoryId).ToHashSet();
            var shareTerritory = targetMemberships.Any(m => requesterTerritoryIds.Contains(m.TerritoryId));
            if (!shareTerritory)
                return Result<UserConnection>.Failure("Este usuário só aceita solicitações de moradores do mesmo território.");
        }

        if (settings.WhoCanAddMe == ConnectionRequestPolicy.ConnectionsOnly)
        {
            var targetAccepted = await _connectionRepository.GetAcceptedConnectionsAsync(targetUserId, cancellationToken);
            var targetAlreadyAddedRequester = targetAccepted.Any(c =>
                c.RequesterUserId == targetUserId && c.TargetUserId == requesterUserId);
            if (!targetAlreadyAddedRequester)
                return Result<UserConnection>.Failure("Este usuário só aceita solicitações de pessoas que ele já adicionou.");
        }

        var connection = UserConnection.CreatePending(
            Guid.NewGuid(),
            requesterUserId,
            targetUserId,
            territoryId,
            DateTime.UtcNow);

        await _connectionRepository.AddAsync(connection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _eventBus.PublishAsync(
            new ConnectionRequestedEvent(
                connection.Id,
                requesterUserId,
                targetUserId,
                territoryId,
                DateTime.UtcNow),
            cancellationToken);

        return Result<UserConnection>.Success(connection);
    }

    public async Task<Result<UserConnection>> AcceptConnectionAsync(
        Guid connectionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionRepository.GetByIdAsync(connectionId, cancellationToken);
        if (connection is null)
            return Result<UserConnection>.Failure("Conexão não encontrada.");

        if (connection.TargetUserId != userId)
            return Result<UserConnection>.Failure("Apenas o destinatário pode aceitar a solicitação.");

        if (connection.Status != ConnectionStatus.Pending)
            return Result<UserConnection>.Failure("A solicitação não está pendente.");

        connection.Accept(DateTime.UtcNow);
        await _connectionRepository.UpdateAsync(connection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _eventBus.PublishAsync(
            new ConnectionAcceptedEvent(
                connection.Id,
                connection.RequesterUserId,
                userId,
                DateTime.UtcNow),
            cancellationToken);

        return Result<UserConnection>.Success(connection);
    }

    public async Task<Result<UserConnection>> RejectConnectionAsync(
        Guid connectionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionRepository.GetByIdAsync(connectionId, cancellationToken);
        if (connection is null)
            return Result<UserConnection>.Failure("Conexão não encontrada.");

        if (connection.TargetUserId != userId)
            return Result<UserConnection>.Failure("Apenas o destinatário pode rejeitar a solicitação.");

        if (connection.Status != ConnectionStatus.Pending)
            return Result<UserConnection>.Failure("A solicitação não está pendente.");

        connection.Reject(DateTime.UtcNow);
        await _connectionRepository.UpdateAsync(connection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<UserConnection>.Success(connection);
    }

    public async Task<OperationResult> RemoveConnectionAsync(
        Guid connectionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionRepository.GetByIdAsync(connectionId, cancellationToken);
        if (connection is null)
            return OperationResult.Failure("Conexão não encontrada.");

        if (connection.RequesterUserId != userId && connection.TargetUserId != userId)
            return OperationResult.Failure("Apenas as partes da conexão podem removê-la.");

        if (connection.Status != ConnectionStatus.Accepted)
            return OperationResult.Failure("Apenas conexões aceitas podem ser removidas.");

        connection.Remove(DateTime.UtcNow);
        await _connectionRepository.UpdateAsync(connection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    public Task<IReadOnlyList<UserConnection>> GetConnectionsAsync(
        Guid userId,
        ConnectionStatus? status,
        CancellationToken cancellationToken)
    {
        return _connectionRepository.GetConnectionsAsync(userId, status, cancellationToken);
    }

    public Task<IReadOnlyList<UserConnection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _connectionRepository.GetPendingRequestsAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Busca usuários por nome de exibição, opcionalmente restrita a um território e papel (Resident/Visitor).
    /// Exclui o próprio usuário e pessoas já conectadas ou com solicitação pendente.
    /// </summary>
    public async Task<IReadOnlyList<User>> SearchUsersAsync(
        Guid currentUserId,
        string? query,
        Guid? territoryId,
        MembershipRole? role,
        int limit,
        CancellationToken cancellationToken)
    {
        var effectiveLimit = Math.Clamp(limit, 1, DefaultSearchLimit);
        IReadOnlyCollection<Guid>? restrictToUserIds = null;

        if (territoryId.HasValue)
        {
            var memberIds = await _membershipRepository.ListUserIdsByTerritoryAsync(territoryId.Value, role, cancellationToken);
            restrictToUserIds = memberIds.Where(id => id != currentUserId).ToList();
        }

        var users = await _userRepository.SearchByDisplayNameAsync(query, restrictToUserIds, effectiveLimit * 2, cancellationToken);

        var excludeIds = new HashSet<Guid> { currentUserId };
        var existing = await _connectionRepository.GetConnectionsAsync(currentUserId, null, cancellationToken);
        foreach (var c in existing)
            excludeIds.Add(c.GetOtherUserId(currentUserId));

        return users.Where(u => !excludeIds.Contains(u.Id)).Take(effectiveLimit).ToList();
    }

    /// <summary>
    /// Retorna sugestões de conexão: membros do território (ou todos) que ainda não são conexões do usuário.
    /// Ordena por "amigos em comum" quando possível (mesmo território).
    /// </summary>
    public async Task<IReadOnlyList<User>> GetSuggestionsAsync(
        Guid currentUserId,
        Guid? territoryId,
        int limit,
        CancellationToken cancellationToken)
    {
        var effectiveLimit = Math.Clamp(limit, 1, DefaultSuggestionsLimit);
        IReadOnlyList<Guid> candidateIds;

        if (territoryId.HasValue)
        {
            candidateIds = await _membershipRepository.ListUserIdsByTerritoryAsync(territoryId.Value, null, cancellationToken);
        }
        else
        {
            var connections = await _connectionRepository.GetAcceptedConnectionsAsync(currentUserId, cancellationToken);
            var connectedIds = connections.Select(c => c.GetOtherUserId(currentUserId)).ToHashSet();
            connectedIds.Add(currentUserId);
            var allUsers = await _userRepository.SearchByDisplayNameAsync(null, null, 500, cancellationToken);
            candidateIds = allUsers.Where(u => !connectedIds.Contains(u.Id)).Select(u => u.Id).ToList();
        }

        var excludeIds = new HashSet<Guid> { currentUserId };
        var existing = await _connectionRepository.GetConnectionsAsync(currentUserId, null, cancellationToken);
        foreach (var c in existing)
            excludeIds.Add(c.GetOtherUserId(currentUserId));

        var suggestedIds = candidateIds.Where(id => !excludeIds.Contains(id)).Take(effectiveLimit * 2).ToList();
        if (suggestedIds.Count == 0)
            return Array.Empty<User>();

        var users = await _userRepository.ListByIdsAsync(suggestedIds, cancellationToken);
        return users.Take(effectiveLimit).ToList();
    }
}
