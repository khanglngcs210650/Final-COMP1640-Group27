using Application.Common.Interfaces;
using AutoMapper;
using Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Queries.ListContribution;

public class ListContributionQueryHandler : IRequestHandler<ListContributionQuery, ErrorOr<IQueryable<ListContributionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ListContributionQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
    }

    public Task<ErrorOr<IQueryable<ListContributionDto>>> Handle(ListContributionQuery request, CancellationToken cancellationToken)
    {
        var contributions = _context.Contributions
            .Include(c => c.CreatedBy).ThenInclude(u => u.Faculty)
            .Include(c => c.Image)
            .Include(c => c.Document)
            .Include(c => c.Period)
            .AsNoTracking();

        var user = _currentUserProvider.GetCurrentUser();

        //For Guest
        if (user == null)
        {
            contributions = contributions.Where(c => c.Status == ContributionStatus.Published);

        }
        else //For authenticated user
        {
            var roles = user.Roles;

            if (roles.Contains("Contributor"))
            {
                contributions = contributions.Where(c => c.CreatedById == user.Id || (c.CreatedBy.FacultyId == user.FacultyId && c.Status == ContributionStatus.Approved));
            }
            else if (roles.Contains("Coordinator"))
            {
                contributions = contributions.Where(c => c.CreatedBy.FacultyId == user.FacultyId);
            }
        }

        var result = _mapper.ProjectTo<ListContributionDto>(contributions);

        return Task.FromResult(result.ToErrorOr());
    }
}