using Araponga.Api.Contracts.Events;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .ValidGuid();

        RuleFor(x => x.Title)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(2000);

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty().WithMessage("StartsAtUtc is required.")
            .Must(date => date > DateTime.UtcNow.AddYears(-1))
            .WithMessage("StartsAtUtc must be in the future (or not more than 1 year in the past).");

        When(x => x.EndsAtUtc.HasValue, () =>
        {
            RuleFor(x => x.EndsAtUtc!.Value)
                .GreaterThan(x => x.StartsAtUtc)
                .WithMessage("EndsAtUtc must be after StartsAtUtc.");
        });

        RuleFor(x => x.Latitude)
            .Latitude();

        RuleFor(x => x.Longitude)
            .Longitude();

        RuleFor(x => x.LocationLabel)
            .MaxLengthWhenNotEmpty(200);

        RuleFor(x => x.AdditionalMediaIds)
            .Must(mediaIds => mediaIds == null || mediaIds.Count <= 5)
            .WithMessage("Maximum 5 additional media items allowed per event.");

        When(x => x.AdditionalMediaIds != null, () =>
        {
            RuleFor(x => x.AdditionalMediaIds!)
                .Must(mediaIds => mediaIds.All(id => id != Guid.Empty))
                .WithMessage("AdditionalMediaIds cannot contain empty GUIDs.");

            RuleFor(x => x.AdditionalMediaIds!)
                .Must(mediaIds => mediaIds.Distinct().Count() == mediaIds.Count)
                .WithMessage("AdditionalMediaIds cannot contain duplicate values.");
        });

        // Validar que CoverMediaId não está duplicado em AdditionalMediaIds
        When(x => x.CoverMediaId.HasValue && x.CoverMediaId.Value != Guid.Empty && 
                  x.AdditionalMediaIds != null && x.AdditionalMediaIds.Count > 0, () =>
        {
            RuleFor(x => x.AdditionalMediaIds!)
                .Must((request, additionalIds) => !additionalIds.Contains(request.CoverMediaId!.Value))
                .WithMessage("CoverMediaId cannot be duplicated in AdditionalMediaIds.");
        });
    }
}
