using Application.DTOs.Admin.Response;
using Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
        }

        /// <summary>
        /// Retrieves aggregated dashboard statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dashboard statistics for the admin frontend.</returns>
        /// <response code="200">Returns the dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminDashboardResponse>> GetDashboard(CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves tourist dashboard statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tourist statistics for the admin frontend.</returns>
        /// <response code="200">Returns the tourist dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/tourists")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminTouristsDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminTouristsDashboardResponse>> GetTouristsDashboard(CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetTouristsDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves tour package dashboard statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tour package statistics for the admin frontend.</returns>
        /// <response code="200">Returns the tour package dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/tour-packages")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminTourPackagesDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminTourPackagesDashboardResponse>> GetTourPackagesDashboard(CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetTourPackagesDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves tour company dashboard statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tour company statistics for the admin frontend.</returns>
        /// <response code="200">Returns the tour company dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/companies")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminCompaniesDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminCompaniesDashboardResponse>> GetCompaniesDashboard(CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetCompaniesDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }
    }
}
