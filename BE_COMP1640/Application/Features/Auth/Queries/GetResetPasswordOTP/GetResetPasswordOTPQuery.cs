using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Queries.GetResetPasswordOTP
{
    public class GetResetPasswordOTPQuery : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }
    }
}
