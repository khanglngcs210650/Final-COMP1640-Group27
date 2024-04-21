using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Queries.ListRole;

public class ListRoleQueryHandler : IRequestHandler<ListRoleQuery, ErrorOr<IQueryable<ListRoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ListRoleQueryHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<ErrorOr<IQueryable<ListRoleDto>>> Handle(ListRoleQuery request, CancellationToken cancellationToken)
    {
        var roleEntities = _context.Roles
            .AsNoTracking();

        var result = _mapper.ProjectTo<ListRoleDto>(roleEntities);

        return Task.FromResult(result.ToErrorOr());
    }
}