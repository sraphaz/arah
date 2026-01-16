using Araponga.Api.Contracts.Events;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.TerritoryId)
            .NotEmpty().WithMessage("TerritoryId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
        });

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
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        When(x => !string.IsNullOrWhiteSpace(x.LocationLabel), () =>
        {
            RuleFor(x => x.LocationLabel)
                .MaximumLength(200).WithMessage("LocationLabel must not exceed 200 characters.");
        });

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
