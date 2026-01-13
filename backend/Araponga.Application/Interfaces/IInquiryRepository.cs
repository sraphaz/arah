using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IInquiryRepository
{
    Task AddAsync(ListingInquiry inquiry, CancellationToken cancellationToken);
    Task<IReadOnlyList<ListingInquiry>> ListByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ListingInquiry>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
}
