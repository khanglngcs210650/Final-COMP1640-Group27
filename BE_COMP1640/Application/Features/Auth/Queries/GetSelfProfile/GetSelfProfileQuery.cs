using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Queries.GetSelfProfile
{
    public class GetSelfProfileQuery : IRequest<ErrorOr<GetSelfProfileDto>>
    {
    }
}
