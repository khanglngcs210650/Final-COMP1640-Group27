using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Queries.ListUser;

public class ListUserQueryHandler : IRequestHandler<ListUserQuery, ErrorOr<IQueryable<ListUserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ListUserQueryHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public Task<ErrorOr<IQueryable<ListUserDto>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
    {
        var userEntities = _context.Users
            .Include(u => u.Faculty)
            .Include(u => u.Roles)
            .Include(u => u.Avatar)
            .AsNoTracking();

        var result = _mapper.ProjectTo<ListUserDto>(userEntities);

        return Task.FromResult(result.ToErrorOr());

    }
}