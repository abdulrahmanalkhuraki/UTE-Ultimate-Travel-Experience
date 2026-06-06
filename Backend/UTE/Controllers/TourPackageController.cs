using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.TourPackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>Lists all tour programs.</summary>
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

        /// <summary>Lists the signed-in company's own programs.</summary>
        [HttpGet("mine")]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetMine(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                return Ok(await _service.GetMineAsync(userId.Value, cancellationToken));
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

        /// <summary>Gets a single program by id.</summary>
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

        /// <summary>Filters published programs.</summary>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> Filter(
            [FromQuery] int? countryId = null,
            [FromQuery] int? cityId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool publishedOnly = true,
            CancellationToken cancellationToken = default)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest(CreateProblemDetails("Invalid search parameters", "minPrice cannot be greater than maxPrice", StatusCodes.Status400BadRequest));

            try
            {
                return Ok(await _service.FilterAsync(countryId, cityId, minPrice, maxPrice, publishedOnly, cancellationToken));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error filtering tour packages");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Creates a new program. Sent as multipart/form-data (carries the main
        /// image and each activity image). The owning company comes from the JWT.
        /// </summary>
        [HttpPost]
        [Authorize]
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
                var created = await _service.CreateAsync(userId.Value, request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
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

        /// <summary>Updates an existing program (multipart/form-data). Owner only.</summary>
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
                var updated = await _service.UpdateAsync(id, userId.Value, request, cancellationToken);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
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

        /// <summary>Deletes a program. Owner only.</summary>
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
                var deleted = await _service.DeleteAsync(id, userId.Value, cancellationToken);
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
