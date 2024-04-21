using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Queries.ListContribution;

public record ListContributionQuery : IRequest<ErrorOr<IQueryable<ListContributionDto>>>;