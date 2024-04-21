using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Commands.PublishContribution;

public class PublishContributionCommand : IRequest<ErrorOr<SuccessResult>>
{
    public Guid Id { get; set; }

    public bool Published { get; set; }
}