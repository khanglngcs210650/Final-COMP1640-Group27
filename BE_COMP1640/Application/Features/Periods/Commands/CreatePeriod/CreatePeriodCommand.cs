using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Periods.Commands.CreatePeriod;

public record CreatePeriodCommand : IRequest<ErrorOr<SuccessResult>>
{

    /// <example>2024</example>
    public int AcademicYear { get; set; }


    /// <example>2024-04-16</example>
    public DateTime FirstSubmissionDeadline { get; set; }

    /// <example>2024-05-20</example>
    public DateTime SecondSubmissionDeadline { get; set; }


}