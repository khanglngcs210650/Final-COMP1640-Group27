using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Periods.Queries.ListPeriod;

public class ListPeriodQueryHandler : IRequestHandler<ListPeriodQuery, ErrorOr<IQueryable<ListPeriodDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ListPeriodQueryHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<ErrorOr<IQueryable<ListPeriodDto>>> Handle(ListPeriodQuery request, CancellationToken cancellationToken)
    {
        var periodEntities = _context.Periods
            .AsNoTracking();


        var result = _mapper.ProjectTo<ListPeriodDto>(periodEntities);

        return Task.FromResult(result.ToErrorOr());
    }
}