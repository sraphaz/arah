using Araponga.Domain.Moderation;

namespace Araponga.Application.Interfaces;

public interface ISanctionRepository
{
    Task AddAsync(Sanction sanction, CancellationToken cancellationToken);
    Task<IReadOnlyList<Sanction>> ListActiveForTargetAsync(
        Guid targetId,
        DateTime referenceUtc,
        CancellationToken cancellationToken);
    Task<bool> HasActiveSanctionAsync(
        Guid targetId,
        Guid territoryId,
        SanctionType type,
        DateTime referenceUtc,
        CancellationToken cancellationToken);
}
