using ErrorOr;
using MediatR;

namespace Application.Features.Comments.Queries.ListComment
{
    public class ListCommentQuery : IRequest<ErrorOr<IQueryable<ListCommentDto>>>
    {
    }
}
