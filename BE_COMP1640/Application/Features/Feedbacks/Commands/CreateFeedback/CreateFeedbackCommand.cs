using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommand : IRequest<ErrorOr<SuccessResult>>
{
    public string Content { get; set; }

    public Guid ContributionId { get; set; }
}