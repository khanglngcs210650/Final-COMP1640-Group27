using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Commands.LoveContribution
{
    public class LoveContributionCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public Guid Id { get; set; }
    }
}
