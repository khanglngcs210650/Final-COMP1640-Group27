namespace Application.Features.Periods.Queries.ListPeriod;

public class ListPeriodDto
{
    public Guid Id { get; set; }

    public int AcademicYear { get; set; }

    public DateTime FirstSubmissionDeadline { get; set; }

    public DateTime SecondSubmissionDeadline { get; set; }

    public string CreatedByEmail { get; set; }

    public string CreatedByFullName { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }
}