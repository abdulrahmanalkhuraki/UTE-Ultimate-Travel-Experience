using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Interfaces.TourPackage;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TourPackageController : ControllerBase
    {
        private readonly ITourPackageService _service;

        public TourPackageController(ITourPackageService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedResponse<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<TourPackageResponse>>> GetAll(
                    [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 20,
                    CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetAllAsync(page, pageSize, cancellationToken));
        }

        [HttpGet("mine/all")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PaginatedResponse<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<TourPackageResponse>>> GetMineAll(
            [FromQuery] TourPackageStatus? status = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetMineAsync(page ?? 1,
                pageSize ?? 20,
                status,
                cancellationToken));
        }

        [HttpGet("mine/{id:int:min(1)}")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourPackageResponse>> GetMineById(int id,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetMineAsync(id, cancellationToken));
        }

        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourPackageResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetAsync(id, cancellationToken));
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(PaginatedResponse<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedResponse<TourPackageResponse>>> Filter(
            [FromQuery] int? countryId = null,
            [FromQuery] int? cityId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest(CreateProblemDetails("Invalid search parameters", "minPrice cannot be greater than maxPrice", StatusCodes.Status400BadRequest));

            return Ok(await _service.FilterAsync(countryId, cityId, minPrice, maxPrice, page, pageSize, cancellationToken));

        }

        [HttpGet("mostWanted")]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetMostWanted(CancellationToken cancellationToken = default)
        {
              return Ok(await _service.GetMostWantedPackagesAsync(cancellationToken));
        }

        [HttpPost]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TourPackageResponse>> Create(
            [FromForm] TourPackageCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }



        [HttpPut("{id:int:min(1)}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TourPackageResponse>> Update(
            int id,
            [FromForm] TourPackageUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var updated = await _service.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        [HttpPost("{id:int:min(1)}/republish")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TourPackageResponse>> Republish(
            int id,
            [FromForm] TourPackageUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.RepublishAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:int:min(1)}/cancel")]
        [Authorize]
        [ProducesResponseType(typeof(ProgramStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProgramStatusResponse>> Cancel(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.CancelAsync(id, cancellationToken));
        }

        [HttpGet("unApproved")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedResponse<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponse<TourPackageResponse>>> GetUnApproved(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetUnApprovedAsync(page, pageSize, cancellationToken));
        }

        [HttpPost("{id:int:min(1)}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProgramStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProgramStatusResponse>> Approve(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.ApproveAsync(id, cancellationToken));
        }

        [HttpPost("{id:int:min(1)}/reject")]
        [Authorize(Roles = "Admin")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProgramStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProgramStatusResponse>> Reject(int id, [FromBody] TourPackageRejectRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.RejectAsync(id, request.Reason, cancellationToken));
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(CreateProblemDetails("Tour package not found", $"Tour package with ID {id} not found", StatusCodes.Status404NotFound));
            return NoContent();
        }

        /// <summary>
        /// Retrieves paginated completed tour packages for the authenticated company.
        /// Each package includes earning analytics, engagement metrics, and publication history.
        /// </summary>
        /// <param name="page">Page number (1-based). Default: 1</param>
        /// <param name="pageSize">Items per page. Default: 20. Max: 100</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated collection of CompletedTourPackageResponse.</returns>
        /// <response code="200">Successfully retrieved completed packages.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/completed")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PaginatedResponse<CompletedTourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<CompletedTourPackageResponse>>> GetMineCompleted(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetMineCompletedAsync(page, pageSize, cancellationToken));
        }

        /// <summary>
        /// Retrieves paginated active tour packages for the authenticated company.
        /// Each package includes time-sensitive information (days until start/registration close) and ratings.
        /// </summary>
        /// <param name="page">Page number (1-based). Default: 1</param>
        /// <param name="pageSize">Items per page. Default: 20. Max: 100</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated collection of ActiveTourPackageResponse.</returns>
        /// <response code="200">Successfully retrieved active packages.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/active")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PaginatedResponse<ActiveTourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<ActiveTourPackageResponse>>> GetMineActive(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetMineActiveAsync(page, pageSize, cancellationToken));
        }

        /// <summary>
        /// Retrieves paginated cancelled tour packages for the authenticated company.
        /// Each package includes cancellation timestamp for audit and display purposes.
        /// </summary>
        /// <param name="page">Page number (1-based). Default: 1</param>
        /// <param name="pageSize">Items per page. Default: 20. Max: 100</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated collection of CancelledTourPackageResponse.</returns>
        /// <response code="200">Successfully retrieved cancelled packages.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/cancelled")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PaginatedResponse<CancelledTourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<CancelledTourPackageResponse>>> GetMineCancelled(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetMineCancelledAsync(page, pageSize, cancellationToken));
        }

        /// <summary>
        /// Retrieves paginated rejected tour packages for the authenticated company.
        /// Each package includes the admin's rejection reason for transparency.
        /// </summary>
        /// <param name="page">Page number (1-based). Default: 1</param>
        /// <param name="pageSize">Items per page. Default: 20. Max: 100</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated collection of RejectedTourPackageResponse.</returns>
        /// <response code="200">Successfully retrieved rejected packages.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/rejected")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PaginatedResponse<RejectedTourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<RejectedTourPackageResponse>>> GetMineRejected(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(CreateProblemDetails(
                    "Invalid Pagination",
                    "Page must be >= 1, PageSize must be between 1 and 100.",
                    StatusCodes.Status400BadRequest));

            return Ok(await _service.GetMineRejectedAsync(page, pageSize, cancellationToken));
        }

        /// <summary>
        /// Retrieves package statistics for the authenticated company's dashboard.
        /// Includes status breakdown and monthly published chart data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Package statistics response.</returns>
        /// <response code="200">Successfully retrieved package statistics.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/stats")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(PackageStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PackageStatsResponse>> GetPackageStats(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetPackageStatsAsync(cancellationToken));
        }

        /// <summary>
        /// Retrieves aggregated rating and review statistics for the authenticated company's dashboard.
        /// Includes weighted average rating, total counts, and monthly rating/review chart data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Rating and review statistics response.</returns>
        /// <response code="200">Successfully retrieved rating and review statistics.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/rate-review-stats")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(RateAndReviewStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RateAndReviewStatsResponse>> GetRateAndReviewStats(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetRateAndReviewStatsAsync(cancellationToken));
        }

        /// <summary>
        /// Retrieves tourist statistics for the authenticated company's dashboard.
        /// Includes unique tourist count, latest bookings, and monthly booking chart data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tourist statistics response.</returns>
        /// <response code="200">Successfully retrieved tourist statistics.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not a tour company or profile not completed.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("mine/tourist-stats")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(TouristStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TouristStatsResponse>> GetTouristStats(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetTouristStatsAsync(cancellationToken));
        }

        #region Helpers

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out var id) && id > 0 ? id : null;
        }

        private ProblemDetails CreateProblemDetails(string title, string detail, int statusCode = StatusCodes.Status500InternalServerError)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = HttpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }

        #endregion
    }
}
