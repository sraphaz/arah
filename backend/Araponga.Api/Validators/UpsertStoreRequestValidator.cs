using Araponga.Api.Contracts.Marketplace;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class UpsertStoreRequestValidator : AbstractValidator<UpsertStoreRequest>
{
    public UpsertStoreRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .NotEmpty().WithMessage("TerritoryId é obrigatório.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Nome de exibição é obrigatório.")
            .MaximumLength(200).WithMessage("Nome de exibição deve ter no máximo 200 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Descrição deve ter no máximo 2000 caracteres.");
        });

        RuleFor(x => x.ContactVisibility)
            .NotEmpty().WithMessage("Visibilidade de contato é obrigatória.")
            .Must(visibility => visibility == "Public" || visibility == "ResidentsOnly" || visibility == "Private")
            .WithMessage("Visibilidade de contato inválida. Valores válidos: Public, ResidentsOnly, Private.");

        When(x => x.Contact != null, () =>
        {
            RuleFor(x => x.Contact!.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Contact!.Email))
                .WithMessage("Email inválido.");

            RuleFor(x => x.Contact!.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Contact!.Phone))
                .WithMessage("Telefone deve ter no máximo 20 caracteres.");

            RuleFor(x => x.Contact!.Website)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.Contact!.Website))
                .WithMessage("Website deve ser uma URL válida.");
        });
    }
}
