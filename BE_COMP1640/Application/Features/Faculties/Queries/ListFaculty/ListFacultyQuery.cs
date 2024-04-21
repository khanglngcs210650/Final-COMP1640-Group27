using ErrorOr;
using MediatR;

namespace Application.Features.Faculties.Queries.ListFaculty;

public record ListFacultyQuery : IRequest<ErrorOr<IQueryable<ListFacultyDto>>>
{

}