using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Models;

public sealed record InquiryCreationResult(
    ItemInquiry Inquiry,
    StoreContactInfo Contact);
