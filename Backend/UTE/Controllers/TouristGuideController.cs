using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;
using Application.Exceptions;
using Application.Interfaces.TouristGuide;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Application.Exceptions.ValidationException;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireCompletedProfile")]
    [Authorize(Roles = "TourCompany")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TouristGuideController : ControllerBase
    {
        private readonly ITouristGuideService _service;
        private readonly ILogger<TouristGuideController> _logger;

        public TouristGuideController(ITouristGuideService service, ILogger<TouristGuideController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists the signed-in company's own guides (مرشدو الشركة) — the list shown
        /// in the program "اختر مرشدك" picker.
        /// </summary>
        [HttpGet("mine")]
        [ProducesResponseType(typeof(IReadOnlyList<TouristGuideResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TouristGuideResponse>>> GetMine(CancellationToken cancellationToken = default)
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
                _logger.LogError(ex, "Error retrieving company's guides");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>Gets a single guide owned by the signed-in company.</summary>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(TouristGuideResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TouristGuideResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                return Ok(await _service.GetAsync(id, userId.Value, cancellationToken));
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Guide not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving guide {GuideId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Creates a guide for the signed-in company (إضافة مرشد). Sent as
        /// multipart/form-data (carries the profile/ID/passport images).
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TouristGuideResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TouristGuideResponse>> Create(
            [FromForm] TouristGuideCreateRequest request,
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error creating guide");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>Updates a guide (multipart/form-data). Company that owns the guide only.</summary>
        [HttpPut("{id:int:min(1)}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TouristGuideResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TouristGuideResponse>> Update(
            int id,
            [FromForm] TouristGuideUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                return Ok(await _service.UpdateAsync(id, userId.Value, request, cancellationToken));
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
                _logger.LogError(ex, "Error updating guide {GuideId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>Removes a guide from the signed-in company. Owner only.</summary>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var deleted = await _service.DeleteAsync(id, userId.Value, cancellationToken);
                if (!deleted)
                    return NotFound(CreateProblemDetails("Guide not found", $"Guide with ID {id} not found", StatusCodes.Status404NotFound));
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
                _logger.LogError(ex, "Error deleting guide {GuideId}", id);
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
