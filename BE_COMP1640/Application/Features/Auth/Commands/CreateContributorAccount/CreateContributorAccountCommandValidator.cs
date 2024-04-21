using FluentValidation;

namespace Application.Features.Auth.Commands.CreateContributorAccount
{
    public class CreateContributorAccountCommandValidator : AbstractValidator<CreateContributorAccountCommand>
    {
        public CreateContributorAccountCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .NotEmpty().WithMessage("First name is required");


            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .NotEmpty().WithMessage("Last name is required");
        }
    }
}
