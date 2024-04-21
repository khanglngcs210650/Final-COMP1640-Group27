using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Faculties.Queries.ListFaculty;

public class ListFacultyQueryHandler : IRequestHandler<ListFacultyQuery, ErrorOr<IQueryable<ListFacultyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ListFacultyQueryHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public Task<ErrorOr<IQueryable<ListFacultyDto>>> Handle(ListFacultyQuery request, CancellationToken cancellationToken)
    {
        var facultyEntities = _context.Faculties
            .AsNoTracking();



        var result = _mapper.ProjectTo<ListFacultyDto>(facultyEntities);

        return Task.FromResult(result.ToErrorOr());
    }
}