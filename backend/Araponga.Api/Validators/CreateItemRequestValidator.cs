using Araponga.Api.Contracts.Marketplace;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .ValidGuid();

        RuleFor(x => x.StoreId)
            .ValidGuid();

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tipo é obrigatório.")
            .Must(type =>
            {
                if (string.IsNullOrWhiteSpace(type))
                    return false;
                var normalized = type.Trim();
                return normalized.Equals("Product", StringComparison.OrdinalIgnoreCase) ||
                       normalized.Equals("Service", StringComparison.OrdinalIgnoreCase);
            })
            .WithMessage("Tipo inválido. Valores válidos: Product, Service (case-insensitive).");

        RuleFor(x => x.Title)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(2000);

        RuleFor(x => x.Category)
            .MaxLengthWhenNotEmpty(100);

        RuleFor(x => x.PricingType)
            .NotEmpty().WithMessage("Tipo de preço é obrigatório.")
            .Must(pricingType =>
            {
                if (string.IsNullOrWhiteSpace(pricingType))
                    return false;
                var normalized = pricingType.Trim();
                return normalized.Equals("Fixed", StringComparison.OrdinalIgnoreCase) ||
                       normalized.Equals("Negotiable", StringComparison.OrdinalIgnoreCase) ||
                       normalized.Equals("Free", StringComparison.OrdinalIgnoreCase);
            })
            .WithMessage("Tipo de preço inválido. Valores válidos: Fixed, Negotiable, Free (case-insensitive).");

        When(x => !string.IsNullOrWhiteSpace(x.PricingType) && 
                  x.PricingType.Trim().Equals("Fixed", StringComparison.OrdinalIgnoreCase) && 
                  x.PriceAmount.HasValue, () =>
        {
            RuleFor(x => x.PriceAmount!.Value)
                .GreaterThan(0).WithMessage("Valor do preço deve ser maior que zero.");
        });

        RuleFor(x => x.Latitude)
            .OptionalLatitude();

        RuleFor(x => x.Longitude)
            .OptionalLongitude();

        RuleFor(x => x.MediaIds)
            .Must(mediaIds => mediaIds == null || mediaIds.Count <= 10)
            .WithMessage("Maximum 10 media items allowed per item.");

        When(x => x.MediaIds != null, () =>
        {
            RuleFor(x => x.MediaIds!)
                .Must(mediaIds => mediaIds.All(id => id != Guid.Empty))
                .WithMessage("MediaIds cannot contain empty GUIDs.");

            RuleFor(x => x.MediaIds!)
                .Must(mediaIds => mediaIds.Distinct().Count() == mediaIds.Count)
                .WithMessage("MediaIds cannot contain duplicate values.");
        });
    }
}
