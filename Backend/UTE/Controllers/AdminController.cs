using Application.DTOs.Admin.Response;
using Application.DTOs.Pagination;
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

        /// <summary>
        /// Retrieves statistics for a single tour company. Restricted to Admin role.
        /// </summary>
        /// <param name="companyId">Identifier of the tour company.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tour company statistics for the admin frontend.</returns>
        /// <response code="200">Returns the tour company dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="404">If the tour company does not exist</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/companies/{companyId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminCompanyDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminCompanyDashboardResponse>> GetCompanyDashboard(int companyId, CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetCompanyDashboardAsync(companyId, cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves paginated per-company financial statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="page">Page number, starting at 1.</param>
        /// <param name="pageSize">Number of companies per page.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Per-company financial statistics for the admin frontend.</returns>
        /// <response code="200">Returns the per-company financial statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/companies/financial")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedResponse<AdminCompanyFinancialResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<AdminCompanyFinancialResponse>>> GetCompaniesFinancial(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetCompaniesFinancialAsync(page, pageSize, cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves financial (profit) dashboard statistics. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Financial statistics for the admin frontend.</returns>
        /// <response code="200">Returns the financial dashboard statistics</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("dashboard/financial")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminFinancialDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdminFinancialDashboardResponse>> GetFinancialDashboard(CancellationToken cancellationToken = default)
        {
            var dashboard = await _adminService.GetFinancialDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }
    }
}
