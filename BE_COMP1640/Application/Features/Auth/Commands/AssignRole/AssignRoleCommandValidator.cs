using FluentValidation;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{

    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }


}