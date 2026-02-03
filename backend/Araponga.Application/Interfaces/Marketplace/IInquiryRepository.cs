using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IInquiryRepository
{
    Task AddAsync(ItemInquiry inquiry, CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemInquiry>> ListByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists inquiries by user with pagination.
    /// </summary>
    Task<IReadOnlyList<ItemInquiry>> ListByUserPagedAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists inquiries by store IDs with pagination.
    /// </summary>
    Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsPagedAsync(
        IReadOnlyCollection<Guid> storeIds,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts inquiries by user.
    /// </summary>
    Task<int> CountByUserAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts inquiries by store IDs.
    /// </summary>
    Task<int> CountByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
}
