using Domain.Enums;

namespace Application.Features.Contributions.Queries.GetContribution;

public class GetContributionDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public ContributionStatus Status { get; set; }

    public int LoveCount { get; set; }

    public bool? IsLoved { get; set; }

    public string CreatedByEmail { get; set; }

    public string CreatedByFullName { get; set; }

    public string CoverImageUrl { get; set; }

    public string DocumentUrl { get; set; }

    public string FacultyId { get; set; }

    public string FacultyName { get; set; }

    public Guid PeriodId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastModifiedAt { get; set; }

}