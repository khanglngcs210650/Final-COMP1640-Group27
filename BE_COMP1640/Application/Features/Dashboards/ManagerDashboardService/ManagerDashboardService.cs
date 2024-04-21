using Application.Common.Interfaces;
using Application.Features.Dashboards.AdminDashboardService;
using Application.Features.Dashboards.AdminDashboardService.DTO;
using Application.Features.Dashboards.CoordinatorDashboardService.DTO;
using Domain.Enums;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboards.ManagerDashboardService
{
    public class ManagerDashboardService : IManagerDashboardService
    {
        private readonly IApplicationDbContext _context;

        public ManagerDashboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<ManagerDashboardDataDto>> GetManagerDashboard(Guid periodId)
        {
            var period = await _context.Periods.FirstOrDefaultAsync(p => p.Id == periodId);

            if (period == null) return Error.NotFound(description: "Period not found");


            var dashboardData = new ManagerDashboardDataDto()
            {
                FacultyRankByContribution = await AdminFacultyRankByContribution(periodId),
                PercentageOfContributionByStatus = await AdminPercentageOfContributionByStatus(periodId),
                PercentageOfFeedbackedContribution = await AdminPercentageOfFeedbackedContribution(periodId),
                NumberOfContributionByStatusWithinFaculty = await AdminNumberOfContributionByStatusWithinFaculty(periodId),
                TotalOfContribution = await AdminTotalOfContributions(periodId),
                TotalOfPublishedContribution = await AdminTotalOfPublishedContributions(periodId),
            };

            return dashboardData;
        }

        private async Task<IDictionary<string, int>> AdminFacultyRankByContribution(Guid periodId)
        {
            var facultyContributions = await _context.Faculties
                .Include(f => f.Members).ThenInclude(u => u.Contributions)
                .Select(f => new
                {
                    FacultyName = f.Name,
                    ContributionCount = f.Members.SelectMany(u => u.Contributions)
                        .Count(c => c.PeriodId == periodId)
                })
                .OrderByDescending(fc => fc.ContributionCount)
                .ToDictionaryAsync(fc => fc.FacultyName, fc => fc.ContributionCount);

            return facultyContributions;
        }

        private async Task<IDictionary<ContributionStatus, double>> AdminPercentageOfContributionByStatus(Guid periodId)
        {
            var totalContributions = await _context.Contributions.CountAsync(c => c.PeriodId == periodId);

            var statusDistribution = await _context.Contributions
                .Where(c => c.PeriodId == periodId)
                .GroupBy(c => c.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Percentage = (double)g.Count() / totalContributions * 100
                })
                .ToDictionaryAsync(g => g.Status, g => g.Percentage);

            return statusDistribution;
        }

        private async Task<double> AdminPercentageOfFeedbackedContribution(Guid periodId)
        {
            var totalContributions = await _context.Contributions.CountAsync(c => c.PeriodId == periodId);

            var contributionsWithFeedback = await _context.Contributions
                .Include(c => c.Feedbacks)
                .CountAsync(c => c.PeriodId == periodId && c.Feedbacks.Any());

            var feedbackPercentage = Math.Round((double)contributionsWithFeedback / totalContributions * 100, 2);

            return feedbackPercentage;
        }



        private async Task<List<NumberOfContributionByStatusWithinFacultyDto>> AdminNumberOfContributionByStatusWithinFaculty(Guid periodId)
        {
            var facultyContributionStatus = await _context.Faculties
                .Include(f => f.Members)
                .ThenInclude(u => u.Contributions.Where(c => c.PeriodId == periodId))
                .Select(f => new NumberOfContributionByStatusWithinFacultyDto()
                {
                    FacultyName = f.Name,
                    PublishedCount = f.Members.SelectMany(u => u.Contributions)
                        .Count(c => c.Status == ContributionStatus.Published),
                    ApprovedCount = f.Members.SelectMany(u => u.Contributions)
                        .Count(c => c.Status == ContributionStatus.Approved),
                    RejectedCount = f.Members.SelectMany(u => u.Contributions)
                        .Count(c => c.Status == ContributionStatus.Rejected)
                }).ToListAsync();

            return facultyContributionStatus;
        }

        private async Task<int> AdminTotalOfPublishedContributions(Guid periodId)
        {
            var publishedContributionsCount = await _context.Contributions
                .CountAsync(c => c.PeriodId == periodId && c.Status == ContributionStatus.Published);

            return publishedContributionsCount;
        }

        private async Task<int> AdminTotalOfContributions(Guid periodId)
        {
            var totalContributionsCount = await _context.Contributions
                .CountAsync(c => c.PeriodId == periodId);

            return totalContributionsCount;
        }

        private async Task<List<TopContributorOfFacultyDto>> TopContributor(Guid periodId, int count)
        {
            var topContributors = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Contributions).ThenInclude(c => c.Period)
                .Include(u => u.Avatar)
                .Where(u => u.Contributions.Any(c => c.PeriodId == periodId))
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
