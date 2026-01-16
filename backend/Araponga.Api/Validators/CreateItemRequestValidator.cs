using Araponga.Api.Contracts.Marketplace;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .NotEmpty().WithMessage("TerritoryId é obrigatório.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("StoreId é obrigatório.");

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
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Descrição deve ter no máximo 2000 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Category), () =>
        {
            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres.");
        });

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

        When(x => x.Latitude.HasValue, () =>
        {
            RuleFor(x => x.Latitude!.Value)
                .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.");
        });

        When(x => x.Longitude.HasValue, () =>
        {
            RuleFor(x => x.Longitude!.Value)
                .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.");
        });

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
