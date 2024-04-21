using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Queries.GetContribution;

public record GetContributionQuery : IRequest<ErrorOr<GetContributionDto>>
{
    public Guid Id { get; set; }

    public GetContributionQuery(Guid id)
    {
        Id = id;
    }
}