using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommand : IRequest<ErrorOr<SuccessResult>>
{
    public string Email { get; set; }

    public Guid RoleId { get; set; }
}