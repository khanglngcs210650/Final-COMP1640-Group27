using FluentValidation;

namespace Application.Features.Contributions.Commands.PublishContribution;

public class PublishContributionCommandValidator : AbstractValidator<PublishContributionCommand>
{
    public PublishContributionCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required");

        RuleFor(command => command.Published).NotNull().WithMessage("Published is required");
    }
}