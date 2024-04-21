using FluentValidation;

namespace Application.Features.Auth.Commands.ToggleActive
{
    public class ToggleActiveCommandValidator : AbstractValidator<ToggleActiveCommand>
    {
        public ToggleActiveCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");
        }
    }
}
