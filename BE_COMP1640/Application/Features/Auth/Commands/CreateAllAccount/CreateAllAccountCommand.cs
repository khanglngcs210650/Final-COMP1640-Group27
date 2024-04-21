using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.CreateAllAccount
{
    public class CreateAllAccountCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid RoleId { get; set; }

        public Guid? FacultyId { get; set; }
    }
}
