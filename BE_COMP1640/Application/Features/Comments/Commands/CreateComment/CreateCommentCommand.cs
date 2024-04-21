using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Content { get; set; }

        public Guid ContributionId { get; set; }
    }
}
