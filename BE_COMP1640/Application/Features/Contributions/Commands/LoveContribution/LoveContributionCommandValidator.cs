using FluentValidation;

namespace Application.Features.Contributions.Commands.LoveContribution
{
    public class LoveContributionCommandValidator : AbstractValidator<LoveContributionCommand>
    {
        public LoveContributionCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required");
        }
    }
}
