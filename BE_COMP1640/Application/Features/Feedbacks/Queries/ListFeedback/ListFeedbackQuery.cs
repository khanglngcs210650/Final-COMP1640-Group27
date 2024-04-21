using ErrorOr;
using MediatR;

namespace Application.Features.Feedbacks.Queries.ListFeedback;

public class ListFeedbackQuery : IRequest<ErrorOr<IQueryable<ListFeedbackDto>>>
{
}