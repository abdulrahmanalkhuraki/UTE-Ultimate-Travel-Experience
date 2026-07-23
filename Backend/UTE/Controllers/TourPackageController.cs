using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.TourPackage;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.TourPackage;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Application.Exceptions.ValidationException;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TourPackageController : ControllerBase
    {
        private readonly ITourPackageService _service;
        private readonly ILogger<TourPackageController> _logger;

        public TourPackageController(ITourPackageService service, ILogger<TourPackageController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.GetAllAsync(cancellationToken));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving tour packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpGet("mine")]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetMine(
            [FromQuery] TourPackageStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                return Ok(await _service.GetMineAsync(status, cancellationToken));
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving company's tour packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TourPackageResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.GetAsync(id, cancellationToken));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Tour package not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> Filter(
            [FromQuery] int? countryId = null,
            [FromQuery] int? cityId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest(CreateProblemDetails("Invalid search parameters", "minPrice cannot be greater than maxPrice", StatusCodes.Status400BadRequest));

            try
            {
                return Ok(await _service.FilterAsync(countryId, cityId, minPrice, maxPrice, cancellationToken));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error filtering tour packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TourPackageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TourPackageResponse>> Create(
            [FromForm] TourPackageCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var created = await _service.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error creating tour package");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var updated = await _service.UpdateAsync(id, request, cancellationToken);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error updating tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var result = await _service.RepublishAsync(id, request, cancellationToken);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error republishing tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpPost("{id:int:min(1)}/cancel")]
        [Authorize]
        [ProducesResponseType(typeof(ProgramStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProgramStatusResponse>> Cancel(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                return Ok(await _service.CancelAsync(id, cancellationToken));
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Tour package not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error cancelling tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpGet("unApproved")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetUnApproved(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.GetUnApprovedAsync(cancellationToken));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving unapproved tour packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpPost("{id:int:min(1)}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProgramStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProgramStatusResponse>> Approve(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.ApproveAsync(id, cancellationToken));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Tour package not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error approving tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
            try
            {
                return Ok(await _service.RejectAsync(id, request.Reason, cancellationToken));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Tour package not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error rejecting tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var deleted = await _service.DeleteAsync(id, cancellationToken);
                if (!deleted)
                    return NotFound(CreateProblemDetails("Tour package not found", $"Tour package with ID {id} not found", StatusCodes.Status404NotFound));
                return NoContent();
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error deleting tour package {PackageId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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
