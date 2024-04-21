using Application.Features.Dashboards.AdminDashboardService.DTO;
using ErrorOr;

namespace Application.Features.Dashboards.AdminDashboardService
{
    public interface IManagerDashboardService
    {
        Task<ErrorOr<ManagerDashboardDataDto>> GetManagerDashboard(Guid periodId);

    }
}
