namespace Araponga.Domain.Connections;

/// <summary>
/// Representa uma conexão (solicitação ou relação estabelecida) entre dois usuários.
/// </summary>
public sealed class UserConnection
{
    public Guid Id { get; }
    public Guid RequesterUserId { get; }
    public Guid TargetUserId { get; }
    public ConnectionStatus Status { get; private set; }
    public Guid? TerritoryId { get; }
    public DateTime RequestedAtUtc { get; }
    public DateTime? RespondedAtUtc { get; private set; }
    public DateTime? RemovedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    private UserConnection(
        Guid id,
        Guid requesterUserId,
        Guid targetUserId,
        ConnectionStatus status,
        Guid? territoryId,
        DateTime requestedAtUtc,
        DateTime? respondedAtUtc,
        DateTime? removedAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (requesterUserId == targetUserId)
            throw new ArgumentException("Requester and target cannot be the same user.", nameof(targetUserId));

        Id = id;
        RequesterUserId = requesterUserId;
        TargetUserId = targetUserId;
        Status = status;
        TerritoryId = territoryId;
        RequestedAtUtc = requestedAtUtc;
        RespondedAtUtc = respondedAtUtc;
        RemovedAtUtc = removedAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static UserConnection CreatePending(
        Guid id,
        Guid requesterUserId,
        Guid targetUserId,
        Guid? territoryId,
        DateTime requestedAtUtc)
    {
        return new UserConnection(
            id,
            requesterUserId,
            targetUserId,
            ConnectionStatus.Pending,
            territoryId,
            requestedAtUtc,
            respondedAtUtc: null,
            removedAtUtc: null,
            createdAtUtc: requestedAtUtc,
            updatedAtUtc: requestedAtUtc);
    }

    /// <summary>
    /// Reconstrói a entidade a partir do persistence (evita máquina de estados na leitura).
    /// </summary>
    public static UserConnection FromPersistence(
        Guid id,
        Guid requesterUserId,
        Guid targetUserId,
        ConnectionStatus status,
        Guid? territoryId,
        DateTime requestedAtUtc,
        DateTime? respondedAtUtc,
        DateTime? removedAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        return new UserConnection(
            id,
            requesterUserId,
            targetUserId,
            status,
            territoryId,
            requestedAtUtc,
            respondedAtUtc,
            removedAtUtc,
            createdAtUtc,
            updatedAtUtc);
    }

    public void Accept(DateTime respondedAtUtc)
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be accepted.");

        Status = ConnectionStatus.Accepted;
        RespondedAtUtc = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void Reject(DateTime respondedAtUtc)
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be rejected.");

        Status = ConnectionStatus.Rejected;
        RespondedAtUtc = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void Remove(DateTime removedAtUtc)
    {
        if (Status != ConnectionStatus.Accepted)
            throw new InvalidOperationException("Only accepted connections can be removed.");

        Status = ConnectionStatus.Removed;
        RemovedAtUtc = removedAtUtc;
        UpdatedAtUtc = removedAtUtc;
    }

    /// <summary>
    /// Retorna o ID do outro usuário da conexão (o par do <paramref name="userId"/>).
    /// </summary>
    public Guid GetOtherUserId(Guid userId)
    {
        if (RequesterUserId == userId) return TargetUserId;
        if (TargetUserId == userId) return RequesterUserId;
        throw new ArgumentException("User is not part of this connection.", nameof(userId));
    }
}
