using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.ToggleActive
{
    public class ToggleActiveCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }
    }
}
