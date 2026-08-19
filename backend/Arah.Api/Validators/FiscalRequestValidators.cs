using Arah.Api.Contracts.Fiscal;
using FluentValidation;

namespace Arah.Api.Validators;

public sealed class UpsertTerritoryFiscalPackBindingRequestValidator
    : AbstractValidator<UpsertTerritoryFiscalPackBindingRequest>
{
    public UpsertTerritoryFiscalPackBindingRequestValidator()
    {
        RuleFor(x => x.PackId)
            .NotEmpty().WithMessage("PackId is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => string.Equals(s, "Off", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(s, "Active", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Status must be Off or Active.");

        When(x => !string.IsNullOrWhiteSpace(x.MunicipalityIbge), () =>
        {
            RuleFor(x => x.MunicipalityIbge!)
                .Matches(@"^\d{7}$")
                .WithMessage("MunicipalityIbge must be exactly 7 digits.");
        });
    }
}

public sealed class UpsertTerritoryPaymentMethodsRequestValidator
    : AbstractValidator<UpsertTerritoryPaymentMethodsRequest>
{
    public UpsertTerritoryPaymentMethodsRequestValidator()
    {
        RuleFor(x => x.Methods)
            .NotNull().WithMessage("Methods is required.");

        RuleForEach(x => x.Methods)
            .Must(m => string.Equals(m, "Pix", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(m, "Card", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(m, "Boleto", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid payment method. Allowed: Pix, Card, Boleto.");

        When(x => !string.IsNullOrWhiteSpace(x.PspProvider), () =>
        {
            RuleFor(x => x.PspProvider!)
                .MaximumLength(64)
                .WithMessage("PspProvider must be at most 64 characters.");
        });
    }
}
