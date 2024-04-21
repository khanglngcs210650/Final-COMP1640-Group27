using Domain.Enums;

namespace Application.Features.Dashboards.AdminDashboardService.DTO
{
    public class ManagerDashboardDataDto
    {
        public IDictionary<string, int> FacultyRankByContribution { get; set; }

        public IDictionary<ContributionStatus, double> PercentageOfContributionByStatus { get; set; }

        public double PercentageOfFeedbackedContribution { get; set; }

        public List<NumberOfContributionByStatusWithinFacultyDto> NumberOfContributionByStatusWithinFaculty { get; set; }

        public int TotalOfContribution { get; set; }

        public int TotalOfPublishedContribution { get; set; }
    }


}
