namespace Application.Features.Dashboards.CoordinatorDashboardService.DTO
{
    public class CoordinatorDashboardDataDto
    {

        public Dictionary<string, double> PercentageOfContributionByStatus { get; set; }

        public string TopContributorFullName { get; set; }

        public int TotalOfContribution { get; set; }

        public int TotalOfPublishedContribution { get; set; }

        public double PercentageOfFeedbackedContribution { get; set; }

        public double ContributionsVsContributorsCorrelation { get; set; }

        public List<TopContributorOfFacultyDto> Top5ContributorOfFaculty { get; set; }
    }
}
