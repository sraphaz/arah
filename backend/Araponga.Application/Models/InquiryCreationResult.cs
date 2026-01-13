using Araponga.Domain.Marketplace;

namespace Araponga.Application.Models;

public sealed record InquiryCreationResult(
    ListingInquiry Inquiry,
    StoreContactInfo Contact);
