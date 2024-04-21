using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand : IRequest<ErrorOr<Success>>
{
    public string Email { get; set; }
    public string Token { get; set; }
}