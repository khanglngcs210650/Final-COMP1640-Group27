using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Queries.ListRole;

public class ListRoleQuery : IRequest<ErrorOr<IQueryable<ListRoleDto>>>
{

}