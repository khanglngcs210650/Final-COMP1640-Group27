using FluentValidation;

namespace Application.Features.Auth.Commands.EditUser
{
    public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");


            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role Id is required");
        }
    }
}
