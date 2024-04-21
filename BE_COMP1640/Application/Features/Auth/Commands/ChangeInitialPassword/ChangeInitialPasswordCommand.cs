using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.ChangeInitialPassword
{
    public class ChangeInitialPasswordCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string ChangeInitialPasswordToken { get; set; }
    }
}
