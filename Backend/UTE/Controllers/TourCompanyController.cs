using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;
using Application.Exceptions;
using Application.Interfaces.TourCompany;
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
        private readonly ILogger<TourCompanyController> _logger;

        public TourCompanyController(ITourCompanyService tourCompanyService, ILogger<TourCompanyController> logger)
        {
            _tourCompanyService = tourCompanyService ?? throw new ArgumentNullException(nameof(tourCompanyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            try
            {
                var company = await _tourCompanyService.GetAsync(id, GetCurrentUserId(), IsAdmin(), cancellationToken);
                return Ok(company);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for tour company ID: {CompanyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Tour company with ID {CompanyId} not found", id);
                return NotFound(CreateProblemDetails("Tour company not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
        [Authorize("TourCompany")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            try
            {
                var created = await _tourCompanyService.CreateAsync(userId.Value, request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating tour company {CompanyName}", request.Name);
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Owner not found while creating tour company {CompanyName}", request.Name);
                return NotFound(CreateProblemDetails("Not Found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict creating tour company {CompanyName}", request.Name);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error creating tour company {CompanyName}", request.Name);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all tour companies.
        /// </summary>
        /// <response code="200">Returns the list of tour companies</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TourCompanyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TourCompanyResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = await _tourCompanyService.GetAllAsync(cancellationToken);
                return Ok(companies);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all tour companies");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            try
            {
                var companies = await _tourCompanyService.FilterAsync(name, location, userId, cancellationToken);
                return Ok(companies);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error filtering tour companies");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
        [Authorize("TourCompany")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            try
            {
                var updated = await _tourCompanyService.UpdateAsync(id, userId.Value, IsAdmin(), request, cancellationToken);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument updating tour company {CompanyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating tour company {CompanyId}", id);
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex, "Forbidden updating tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Tour company not found for update: {CompanyId}", id);
                return NotFound(CreateProblemDetails("Tour company not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict updating tour company {CompanyId}", id);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating tour company {CompanyId}", id);
                return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error updating tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
        [Authorize("TourCompany")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            try
            {
                var deleted = await _tourCompanyService.DeleteAsync(id, userId.Value, IsAdmin(), cancellationToken);

                if (!deleted)
                {
                    _logger.LogWarning("Tour company not found for deletion: {CompanyId}", id);
                    return NotFound(CreateProblemDetails(
                        "Tour company not found",
                        $"Tour company with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                _logger.LogInformation("Tour company {CompanyId} successfully deleted", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument deleting tour company {CompanyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex, "Forbidden deleting tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error deleting tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            try
            {
                var companies = await _tourCompanyService.GetPendingAsync(cancellationToken);
                return Ok(companies);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving pending tour companies");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            try
            {
                var company = await _tourCompanyService.ApproveAsync(id, cancellationToken);
                return Ok(company);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument approving tour company {CompanyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Tour company not found for approval: {CompanyId}", id);
                return NotFound(CreateProblemDetails("Tour company not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error approving tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            if (!ModelState.IsValid)
                return BadRequest(CreateValidationProblemDetails());

            try
            {
                var company = await _tourCompanyService.RejectAsync(id, request.Reason, cancellationToken);
                return Ok(company);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument rejecting tour company {CompanyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Tour company not found for rejection: {CompanyId}", id);
                return NotFound(CreateProblemDetails("Tour company not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error rejecting tour company {CompanyId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
