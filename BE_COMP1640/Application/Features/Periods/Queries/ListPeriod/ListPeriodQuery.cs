using ErrorOr;
using MediatR;

namespace Application.Features.Periods.Queries.ListPeriod;

public class ListPeriodQuery : IRequest<ErrorOr<IQueryable<ListPeriodDto>>>
{

}