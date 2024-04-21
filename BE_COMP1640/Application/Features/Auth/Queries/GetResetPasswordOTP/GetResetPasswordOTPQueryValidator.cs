using FluentValidation;

namespace Application.Features.Auth.Queries.GetResetPasswordOTP
{
    public class GetResetPasswordOTPQueryValidator : AbstractValidator<GetResetPasswordOTPQuery>
    {
        public GetResetPasswordOTPQueryValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");
        }
    }
}
