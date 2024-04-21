using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ErrorOr<SuccessResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ErrorOr<SuccessResult>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) return Error.NotFound(description: "Email not found");

            var otpVerified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider,
                "ResetPasswordPurpose", request.OTP);

            if (!otpVerified) return Error.Validation(description: "Invalid OTP");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded) return Error.Unexpected(description: "Reset password failed, please try again");


            return new SuccessResult(title: "Reset password successfully, now you can login with new password");
        }
    }
}
