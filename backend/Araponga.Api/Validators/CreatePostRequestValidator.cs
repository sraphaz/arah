using Araponga.Api.Contracts.Feed;
using Araponga.Domain.Feed;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(4000).WithMessage("Content must not exceed 4000 characters.");

        RuleFor(x => x.Type)
            .Must(type => !string.IsNullOrWhiteSpace(type) && Enum.TryParse<PostType>(type, true, out _))
            .WithMessage("Invalid post type. Valid values: General, Alert, Event.");

        RuleFor(x => x.Visibility)
            .Must(visibility => !string.IsNullOrWhiteSpace(visibility) && Enum.TryParse<PostVisibility>(visibility, true, out _))
            .WithMessage("Invalid visibility setting. Valid values: Public, ResidentsOnly.");

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
    }
}
