namespace Application.Features.Dashboards.CoordinatorDashboardService.DTO
{
    public class TopContributorOfFacultyDto
    {

        public string FullName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string FacultyName { get; set; }

        public int ContributionCount { get; set; }
    }
}

