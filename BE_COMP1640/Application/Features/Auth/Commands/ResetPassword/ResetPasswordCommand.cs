using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public string OTP { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
