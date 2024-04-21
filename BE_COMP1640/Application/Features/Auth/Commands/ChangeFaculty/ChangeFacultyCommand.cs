using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.ChangeFaculty
{
    public class ChangeFacultyCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public Guid FacultyId { get; set; }
    }
}
