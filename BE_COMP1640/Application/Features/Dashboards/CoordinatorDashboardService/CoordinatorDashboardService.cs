using Application.Common.Interfaces;
using Application.Features.Dashboards.CoordinatorDashboardService.DTO;
using Domain.Enums;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboards.CoordinatorDashboardService
{
    public class CoordinatorDashboardService : ICoordinatorDashboardService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CoordinatorDashboardService(IApplicationDbContext context,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ErrorOr<CoordinatorDashboardDataDto>> GetCoordinatorDashboard(Guid periodId)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();
            if (currentUser == null) return Error.Unauthorized(description: "Please login first");

            var period = await _context.Periods.FirstOrDefaultAsync(p => p.Id == periodId);
            if (period == null) return Error.NotFound(description: "Period not found");

            var isFacultyIdExist = await _context.Faculties.AnyAsync(f => f.Id == currentUser.FacultyId);

            if (!isFacultyIdExist) return Error.NotFound("Faculty Id of the current coordinator not found");

            var facultyId = currentUser.FacultyId;


            var dashboardData = new CoordinatorDashboardDataDto()
            {
                PercentageOfContributionByStatus = await PercentageOfContributionByStatus(facultyId, periodId),
                TopContributorFullName = await TopContributorFullName(facultyId, periodId),
                TotalOfContribution = await TotalContributionsCount(facultyId, periodId),
                TotalOfPublishedContribution = await TotalPublishedContributionsCount(facultyId, periodId),
                PercentageOfFeedbackedContribution = await PercentageOfFeedbackedContribution(facultyId, periodId),
                ContributionsVsContributorsCorrelation = await GetContributionsVsContributorsCorrelation(facultyId, periodId),
                Top5ContributorOfFaculty = await TopContributors(facultyId, periodId, 5)
            };

            return dashboardData;
        }

        private async Task<Dictionary<string, double>> PercentageOfContributionByStatus(Guid facultyId, Guid periodId)
        {
            // Get the total number of contributions for the specified faculty and period
            var totalContributions = await _context.Contributions
                .CountAsync(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId);

            // Group contributions by status and calculate the percentage for each status
            var contributionsByStatus = await _context.Contributions
                .Where(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId)
                .GroupBy(c => c.Status)
                .Select(g => new ContributionStatusPercentage
                {
                    Status = g.Key.ToString(),
                    Percentage = (double)g.Count() / totalContributions * 100
                })
                .ToListAsync();

            // Calculate the total percentage of all statuses
            var totalPercentage = contributionsByStatus.Sum(item => item.Percentage);

            // Round the total percentage to two decimal places
            totalPercentage = Math.Round(totalPercentage, 2);

            // Adjust the percentage of each status so that the total percentage is 100%
            contributionsByStatus.ForEach(item => item.Percentage = item.Percentage / totalPercentage * 100);

            // Create a dictionary from the adjusted list
            var adjustedPercentages = contributionsByStatus.ToDictionary(item => item.Status.ToString(), item => item.Percentage);

            return adjustedPercentages;
        }

        private async Task<string> TopContributorFullName(Guid facultyId, Guid periodId)
        {
            var topContributor = await _context.Users
                .Include(u => u.Contributions)
                .Where(u => u.FacultyId == facultyId)
                .OrderByDescending(u => u.Contributions.Count)
                .FirstOrDefaultAsync();

            if (topContributor == null) return "There is no contributor in this period";

            return $"{topContributor.FirstName + " " + topContributor.LastName}";
        }

        private async Task<int> TotalContributionsCount(Guid facultyId, Guid periodId)
        {
            var totalContributionsCount = await _context.Contributions
                .Include(c => c.CreatedBy)
                .CountAsync(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId);

            return totalContributionsCount;
        }

        private async Task<int> TotalPublishedContributionsCount(Guid facultyId, Guid periodId)
        {
            var publishedContributionsCount = await _context.Contributions.Include(c => c.CreatedBy)
                .CountAsync(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId && c.Status == ContributionStatus.Published);

            return publishedContributionsCount;
        }

        private async Task<double> PercentageOfFeedbackedContribution(Guid facultyId, Guid periodId)
        {
            var totalContributions = await _context.Contributions
                .Include(c => c.CreatedBy)
                .CountAsync(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId);

            var contributionsWithFeedback = await _context.Contributions
                .Include(c => c.Feedbacks)
                .Where(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId && c.Feedbacks.Any())
                .CountAsync();

            double feedbackPercentage = totalContributions > 0 ? Math.Round((double)contributionsWithFeedback / totalContributions * 100, 2) : 0;

            return feedbackPercentage;
        }

        private async Task<double> GetContributionsVsContributorsCorrelation(Guid facultyId, Guid periodId)
        {
            var totalContributions = await _context.Contributions
                .Include(c => c.CreatedBy)
                .CountAsync(c => c.CreatedBy.FacultyId == facultyId && c.PeriodId == periodId);

            var totalContributors = await _context.Users
                .CountAsync(u => u.FacultyId == facultyId);

            double correlation = totalContributors > 0 ? (double)totalContributions / totalContributors : 0;

            return correlation;
        }

        private async Task<List<TopContributorOfFacultyDto>> TopContributors(Guid facultyId, Guid periodId, int count)
        {
            var topContributors = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Contributions).ThenInclude(c => c.Period)
                .Include(u => u.Avatar)
                .Where(u => u.FacultyId == facultyId && u.Contributions.Any(c => c.PeriodId == periodId))
                .OrderByDescending(u => u.Contributions.Count)
                .Take(count)
                .Select(u => new TopContributorOfFacultyDto()
                {
                    Email = u.Email,
                    FullName = $"{u.FirstName} {u.LastName}",
                    AvatarUrl = u.Avatar.UrlFilePath,
                    ContributionCount = u.Contributions.Count,
                    FacultyName = u.Faculty.Name
                })
                .ToListAsync();

            return topContributors;
        }
    }
}
