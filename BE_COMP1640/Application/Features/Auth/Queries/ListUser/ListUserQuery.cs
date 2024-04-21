using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Queries.ListUser;

public class ListUserQuery : IRequest<ErrorOr<IQueryable<ListUserDto>>>
{
}