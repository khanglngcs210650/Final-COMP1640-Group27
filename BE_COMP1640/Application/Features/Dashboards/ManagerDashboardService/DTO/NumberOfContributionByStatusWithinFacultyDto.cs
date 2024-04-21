namespace Application.Features.Dashboards.AdminDashboardService.DTO
{
    public class NumberOfContributionByStatusWithinFacultyDto
    {
        public string FacultyName { get; set; }

        public int PublishedCount { get; set; }

        public int ApprovedCount { get; set; }

        public int RejectedCount { get; set; }
    }
}
