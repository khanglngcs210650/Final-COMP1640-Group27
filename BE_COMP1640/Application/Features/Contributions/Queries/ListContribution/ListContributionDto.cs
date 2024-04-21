using Domain.Enums;

namespace Application.Features.Contributions.Queries.ListContribution;

public class ListContributionDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public ContributionStatus Status { get; set; }

    public string CreatedByEmail { get; set; }

    public string CreatedByFullName { get; set; }

    public string FacultyName { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? PeriodId { get; set; }

    public int AcademicYear { get; set; }

    public DateTime FirstSubmissionDeadline { get; set; }
    public DateTime SecondSubmissionDeadline { get; set; }

    public string CoverImageUrl { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

}