using Application.Features.Dashboards.CoordinatorDashboardService.DTO;
using ErrorOr;

namespace Application.Features.Dashboards.CoordinatorDashboardService
{
    public interface ICoordinatorDashboardService
    {
        Task<ErrorOr<CoordinatorDashboardDataDto>> GetCoordinatorDashboard(Guid periodId);
    }
}
