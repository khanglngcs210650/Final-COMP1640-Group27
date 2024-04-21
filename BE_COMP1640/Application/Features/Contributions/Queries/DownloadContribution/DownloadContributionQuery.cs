using ErrorOr;
using MediatR;

namespace Application.Features.Contributions.Queries.DownloadContribution
{
    public class DownloadContributionQuery : IRequest<ErrorOr<byte[]>>
    {

    }
}
