
using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Queries.GetContribution;

public class GetContributionQueryHandler : IRequestHandler<GetContributionQuery, ErrorOr<GetContributionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetContributionQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
    }


    public async Task<ErrorOr<GetContributionDto>> Handle(GetContributionQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();

        //Find contribution by Id
        var contributionEntity = await _context.Contributions
            .Include(c => c.Document)
            .Include(c => c.Image)
            .Include(c => c.Ratings)
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        //Check if null
        if (contributionEntity == null) return Error.NotFound(description: "Contribution not found");

        var contributionDto = _mapper.Map<GetContributionDto>(contributionEntity);


        if (currentUser != null && contributionEntity.Ratings.Any(r => r.CreatedById == currentUser.Id))
        {
            contributionDto.IsLoved = true;
        }
        else
        {
            contributionDto.IsLoved = false;
        }

        return contributionDto;
    }
}