using Application.Features.Dashboards.AdminDashboardService;
using Application.Features.Dashboards.CoordinatorDashboardService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ApiController
    {
        private readonly IManagerDashboardService _managerDashboardService;
        private readonly ICoordinatorDashboardService _coordinatorDashboardService;

        public DashboardsController(IManagerDashboardService managerDashboardService,
            ICoordinatorDashboardService coordinatorDashboardService)
        {
            _managerDashboardService = managerDashboardService;
            _coordinatorDashboardService = coordinatorDashboardService;
        }


        /// <summary>
        ///   Get Manager dashboard data
        /// </summary>
        [HttpGet("Manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ManagerDashboard([FromQuery] Guid periodId)
        {
            var result = await _managerDashboardService.GetManagerDashboard(periodId);

            return result.Match(
                value => base.Ok(value),
                Problem);
        }



        /// <summary>
        ///   Get Coordinator dashboard data
        /// </summary>
        [HttpGet("Coordinator")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> CoordinatorDashboard([FromQuery] Guid periodId)
        {
            var result = await _coordinatorDashboardService.GetCoordinatorDashboard(periodId);

            return result.Match(
                value => base.Ok(value),
                Problem);
        }

    }
}
