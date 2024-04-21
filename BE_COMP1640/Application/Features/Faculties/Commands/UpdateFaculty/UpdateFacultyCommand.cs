using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Faculties.Commands.EditFaculty
{
    public class UpdateFacultyCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
