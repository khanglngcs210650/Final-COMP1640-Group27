using FluentValidation;

namespace Application.Features.Contributions.Commands.ApproveContribution;

public class ApproveContributionCommandValidator : AbstractValidator<ApproveContributionCommand>
{
    public ApproveContributionCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required");

        RuleFor(command => command.Approved).NotNull().WithMessage("Approved is required");
    }
}