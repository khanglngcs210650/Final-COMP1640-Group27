using FluentValidation;

namespace Application.Features.Auth.Commands.ChangeInitialPassword
{
    public class ChangeInitialPasswordCommandValidator : AbstractValidator<ChangeInitialPasswordCommand>
    {
        public ChangeInitialPasswordCommandValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");


            RuleFor(x => x.ChangeInitialPasswordToken)
                .NotEmpty().WithMessage("Change Initial Password Token is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches("[0-9]").WithMessage("New password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("New password confirmation is required")
                .Equal(x => x.NewPassword).WithMessage("New password confirmation must match the new password");
        }
    }
}
