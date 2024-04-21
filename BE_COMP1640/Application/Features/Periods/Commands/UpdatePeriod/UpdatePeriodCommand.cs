using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Periods.Commands.UpdatePeriod;

public record UpdatePeriodCommand : IRequest<ErrorOr<SuccessResult>>
{
    public Guid Id { get; set; }

    /// <example>2024-06-16</example>
    public DateTime FirstSubmissionDeadline { get; set; }

    /// <example>2024-07-16</example>
    public DateTime SecondSubmissionDeadline { get; set; }


    public UpdatePeriodCommand(Guid id, DateTime firstSubmissionDeadline, DateTime secondSubmissionDeadline)
    {
        Id = id;
        FirstSubmissionDeadline = firstSubmissionDeadline;
        SecondSubmissionDeadline = secondSubmissionDeadline;
    }
}