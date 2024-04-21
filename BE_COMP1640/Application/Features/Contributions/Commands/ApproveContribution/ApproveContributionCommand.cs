using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Commands.ApproveContribution;

public class ApproveContributionCommand : IRequest<ErrorOr<SuccessResult>>
{
    public Guid Id { get; set; }

    public bool Approved { get; set; }
}