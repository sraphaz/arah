namespace Araponga.Domain.Governance;

/// <summary>
/// Representa um voto individual em uma votação.
/// </summary>
public sealed class Vote
{
    public Vote(
        Guid id,
        Guid votingId,
        Guid userId,
        string selectedOption,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            throw new ArgumentException("Selected option is required.", nameof(selectedOption));
        }

        Id = id;
        VotingId = votingId;
        UserId = userId;
        SelectedOption = selectedOption.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Identificador único do voto.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador da votação.
    /// </summary>
    public Guid VotingId { get; }

    /// <summary>
    /// Identificador do usuário que votou.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Opção selecionada pelo usuário.
    /// </summary>
    public string SelectedOption { get; }

    /// <summary>
    /// Data/hora UTC em que o voto foi registrado.
    /// </summary>
    public DateTime CreatedAtUtc { get; }
}
