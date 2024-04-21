using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.EditUser
{
    public class EditUserCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public Guid RoleId { get; set; }

        public Guid? FacultyId { get; set; }
    }
}
