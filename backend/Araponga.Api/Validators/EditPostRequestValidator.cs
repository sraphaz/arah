using Araponga.Api.Contracts.Feed;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class EditPostRequestValidator : AbstractValidator<EditPostRequest>
{
    public EditPostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(4000).WithMessage("Content must not exceed 4000 characters.");

        RuleFor(x => x.GeoAnchors)
            .Must(anchors => anchors == null || anchors.Count <= 50)
            .WithMessage("Maximum 50 geo anchors allowed.");

        When(x => x.GeoAnchors != null, () =>
        {
            RuleForEach(x => x.GeoAnchors)
                .ChildRules(anchor =>
                {
                    anchor.RuleFor(a => a.Latitude)
                        .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

                    anchor.RuleFor(a => a.Longitude)
                        .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
                });
        });

        RuleFor(x => x.MediaIds)
            .Must(mediaIds => mediaIds == null || mediaIds.Count <= 10)
            .WithMessage("Maximum 10 media items allowed per post.");

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
