using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Queries.GetResetPasswordOTP
{
    public class GetResetPasswordOTPQueryHandler : IRequestHandler<GetResetPasswordOTPQuery, ErrorOr<SuccessResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public GetResetPasswordOTPQueryHandler(UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ErrorOr<SuccessResult>> Handle(GetResetPasswordOTPQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) return Error.NotFound(description: "Email not found");

            var otp = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPasswordPurpose");


            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email,
                "Password Reset OTP from MagazineHub",
                $"Please enter the following OTP to reset your password: <strong>{otp}</strong>"));

            return new SuccessResult(title: "We have sent you an email that contains the OTP, please check your email");
        }
    }
}
