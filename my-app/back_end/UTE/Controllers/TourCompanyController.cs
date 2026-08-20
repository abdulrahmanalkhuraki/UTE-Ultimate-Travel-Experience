using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;
using Application.Exceptions;
using Application.Interfaces.TourCompany;
using Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using ValidationException = Application.Exceptions.ValidationException;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TourCompanyController : ControllerBase
    {
        private readonly ITourCompanyService _tourCompanyService;

        public TourCompanyController(ITourCompanyService tourCompanyService)
        {
            _tourCompanyService = tourCompanyService ?? throw new ArgumentNullException(nameof(tourCompanyService));
        }


        /// <summary>
        /// Retrieves a specific tour company by ID.
        /// </summary>
        /// <param name="id">Tour company ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested tour company</returns>
        /// <response code="200">Returns the requested tour company</response>
        /// <response code="404">If the tour company is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var company = await _tourCompanyService.GetAsync(id, cancellationToken);
            return Ok(company);
        }

        /// <summary>
        /// Retrieves tour company That Attached To The Current User.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested tour company</returns>
        /// <response code="200">Returns the requested tour company</response>
        /// <response code="409">If the user doesn't attached to a Tour Company</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("mine")]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> GetMine(CancellationToken cancellationToken = default)
        {
            var company = await _tourCompanyService.GetMineAsync(cancellationToken);
            return Ok(company);
        }

        /// <summary>
        /// Creates a new tour company for the authenticated owner.
        /// Sent as multipart/form-data (carries the logo and tourism-license image).
        /// The owner is taken from the JWT.
        /// </summary>
        /// <param name="request">Tour company creation request (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created tour company</returns>
        /// <response code="201">Returns the newly created tour company</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the owner is not found</response>
        /// <response code="409">If a conflict occurs (duplicate company)</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> Create(
            [FromForm] TourCompanyCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var created = await _tourCompanyService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        /// <summary>
        /// Retrieves all tour companies.
        /// </summary>
        /// <response code="200">Returns the list of tour companies</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<TourCompanyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TourCompanyResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var companies = await _tourCompanyService.GetAllAsync(cancellationToken);
            return Ok(companies);          
        }

        /// <summary>
        /// Searches tour companies by name, location, or owner.
        /// </summary>
        /// <param name="name">Company name filter (partial match)</param>
        /// <param name="location">Location filter (partial match)</param>
        /// <param name="userId">Owner user ID filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns matching tour companies</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<TourCompanyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TourCompanyResponse>>> Filter(
            [FromQuery] string? name = null,
            [FromQuery] string? location = null,
            [FromQuery] int? userId = null,
            CancellationToken cancellationToken = default)
        {
            var companies = await _tourCompanyService.FilterAsync(name, location, userId, cancellationToken);
            return Ok(companies);
        }

        /// <summary>
        /// Updates an existing tour company (partial update). Only the owner or an admin may update.
        /// Sent as multipart/form-data. Only the fields provided are changed; an image is replaced
        /// only when a new file is uploaded.
        /// </summary>
        /// <param name="id">Tour company ID</param>
        /// <param name="request">Fields to update (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the updated tour company</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the caller does not own the company</response>
        /// <response code="404">If the tour company is not found</response>
        /// <response code="409">If a conflict or concurrency issue occurs</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> Update(
            int id,
            [FromForm] TourCompanyUpdateRequest request,
            CancellationToken cancellationToken = default)
        {

            var updated = await _tourCompanyService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a tour company. Only the owner or an admin may delete.
        /// </summary>
        /// <param name="id">Tour company ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">If the tour company was successfully deleted</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the caller does not own the company</response>
        /// <response code="404">If the tour company is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {

                var deleted = await _tourCompanyService.DeleteAsync(id, cancellationToken);
                return NoContent();
        }

        /// <summary>
        /// Lists all tour companies awaiting approval. Admin only.
        /// </summary>
        /// <response code="200">Returns the pending tour companies</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<TourCompanyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TourCompanyResponse>>> GetPending(CancellationToken cancellationToken = default)
        {
            var companies = await _tourCompanyService.GetPendingAsync(cancellationToken);
            return Ok(companies);
        }



        /// <summary>
        /// Retrieves dashboard information for the authenticated tour company.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="TourCompanyDashboardResponse"/> containing the tour company's dashboard data.</returns>
        /// <response code="200">Returns the dashboard information for the authenticated tour company.</response>
        /// <response code="401">The request is not authenticated. The user must be logged in to access this endpoint.</response>
        /// <response code="403">The authenticated user does not have the 'TourCompany' role required to access this endpoint.</response>
        /// <response code="404">The authenticated user is not associated with any tour company.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpGet("MyDashboard")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles ="TourCompany")]
        [ProducesResponseType(typeof(TourCompanyDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyDashboardResponse>> MyDashboard(CancellationToken cancellationToken = default)
        {
            return Ok(await _tourCompanyService.MyDashboard(cancellationToken));
        }

        /// <summary>
        /// Approves a pending tour company, making it publicly visible. Admin only.
        /// </summary>
        /// <param name="id">Tour company ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the approved tour company</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an admin</response>
        /// <response code="404">If the tour company is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("{id:int:min(1)}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> Approve(int id, CancellationToken cancellationToken = default)
        {
            var company = await _tourCompanyService.ApproveAsync(id, cancellationToken);
            return Ok(company);
        }

        /// <summary>
        /// Rejects a tour company (with a reason) so it stays hidden from the public. Admin only.
        /// </summary>
        /// <param name="id">Tour company ID</param>
        /// <param name="request">The rejection reason shown to the owner</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the rejected tour company</response>
        /// <response code="400">If the reason is missing or invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an admin</response>
        /// <response code="404">If the tour company is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("{id:int:min(1)}/reject")]
        [Authorize(Roles = "Admin")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(TourCompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TourCompanyResponse>> Reject(int id, [FromBody] TourCompanyRejectRequest request, CancellationToken cancellationToken = default)
        {
            var company = await _tourCompanyService.RejectAsync(id, request.Reason, cancellationToken);
            return Ok(company);
        }

        #region Private Helper Methods

        private bool IsAdmin() => User.IsInRole("Admin");

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

        private ValidationProblemDetails CreateValidationProblemDetails()
        {
            return new ValidationProblemDetails(ModelState)
            {
                Type = "https://httpstatuses.com/400",
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            };
        }

        #endregion
    }
}
