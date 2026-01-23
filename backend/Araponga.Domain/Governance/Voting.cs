namespace Araponga.Domain.Governance;

/// <summary>
/// Representa uma votação comunitária para decisões coletivas.
/// </summary>
public sealed class Voting
{
    public Voting(
        Guid id,
        Guid territoryId,
        Guid createdByUserId,
        VotingType type,
        string title,
        string description,
        IReadOnlyList<string> options,
        VotingVisibility visibility,
        VotingStatus status,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (options is null || options.Count < 2)
        {
            throw new ArgumentException("At least 2 options are required.", nameof(options));
        }

        Id = id;
        TerritoryId = territoryId;
        CreatedByUserId = createdByUserId;
        Type = type;
        Title = title.Trim();
        Description = description.Trim();
        Options = options;
        Visibility = visibility;
        Status = status;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único da votação.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde a votação ocorre.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador do usuário que criou a votação.
    /// </summary>
    public Guid CreatedByUserId { get; }

    /// <summary>
    /// Tipo da votação.
    /// </summary>
    public VotingType Type { get; }

    /// <summary>
    /// Título da votação.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Descrição da votação.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Lista de opções de voto.
    /// </summary>
    public IReadOnlyList<string> Options { get; }

    /// <summary>
    /// Visibilidade da votação (quem pode votar).
    /// </summary>
    public VotingVisibility Visibility { get; }

    /// <summary>
    /// Status atual da votação.
    /// </summary>
    public VotingStatus Status { get; private set; }

    /// <summary>
    /// Data/hora UTC em que a votação começa (opcional, se null começa imediatamente).
    /// </summary>
    public DateTime? StartsAtUtc { get; }

    /// <summary>
    /// Data/hora UTC em que a votação termina (opcional, se null não tem prazo).
    /// </summary>
    public DateTime? EndsAtUtc { get; }

    /// <summary>
    /// Data/hora UTC de criação da votação.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data/hora UTC da última atualização da votação.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Fecha a votação (não aceita mais votos).
    /// </summary>
    public void Close()
    {
        if (Status != VotingStatus.Open)
        {
            throw new InvalidOperationException("Only open votings can be closed.");
        }

        Status = VotingStatus.Closed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Aprova a votação (resultado será aplicado).
    /// </summary>
    public void Approve()
    {
        if (Status != VotingStatus.Closed)
        {
            throw new InvalidOperationException("Only closed votings can be approved.");
        }

        Status = VotingStatus.Approved;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejeita a votação (resultado não será aplicado).
    /// </summary>
    public void Reject()
    {
        if (Status != VotingStatus.Closed)
        {
            throw new InvalidOperationException("Only closed votings can be rejected.");
        }

        Status = VotingStatus.Rejected;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Abre a votação para aceitar votos.
    /// </summary>
    public void Open()
    {
        if (Status != VotingStatus.Draft)
        {
            throw new InvalidOperationException("Only draft votings can be opened.");
        }

        Status = VotingStatus.Open;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
