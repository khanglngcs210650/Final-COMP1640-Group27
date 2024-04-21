using FluentValidation;

namespace Application.Features.Auth.Commands.ChangeFaculty
{
    public class ChangeFacultyCommandValidator : AbstractValidator<ChangeFacultyCommand>
    {
        public ChangeFacultyCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.FacultyId)
                .NotEmpty().WithMessage("FacultyId is required.");
        }
    }
}
