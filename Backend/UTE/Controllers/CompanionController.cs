using System.Net.Mime;
using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.Exceptions;
using Application.Interfaces.Companion;
using Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tourist")]
    [Produces(MediaTypeNames.Application.Json)]
    public class CompanionController : ControllerBase
    {
        private readonly ICompanionService _companionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CompanionController> _logger;

        public CompanionController(ICompanionService companionService, ICurrentUserService currentUserService, ILogger<CompanionController> logger)
        {
            _companionService = companionService ?? throw new ArgumentNullException(nameof(companionService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Retrieves all companions for the authenticated tourist.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of the tourist's companions.</returns>
        /// <response code="200">Returns the list of companions.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a tourist.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CompanionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<CompanionResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            return Ok(await _companionService.GetAllAsync(userId.Value, cancellationToken));
        }

        /// <summary>Retrieves a specific companion by ID.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The requested companion.</returns>
        /// <response code="200">Returns the requested companion.</response>
        /// <response code="400">If the ID is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the companion does not belong to the user.</response>
        /// <response code="404">If the companion is not found.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(CompanionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanionResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            return Ok(await _companionService.GetAsync(id, userId.Value, cancellationToken));
        }

        /// <summary>Creates a new companion for the authenticated tourist.</summary>
        /// <param name="request">The companion creation payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newly created companion.</returns>
        /// <response code="201">Returns the newly created companion.</response>
        /// <response code="400">If validation fails.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a tourist.</response>
        /// <response code="404">If a referenced country or city is not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CompanionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanionResponse>> Create(
            [FromForm] CompanionCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            var created = await _companionService.CreateAsync(userId.Value, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);

        }

        /// <summary>Updates an existing companion.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="request">The companion update payload. All fields are optional for partial update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated companion.</returns>
        /// <response code="200">Returns the updated companion.</response>
        /// <response code="400">If validation fails or the ID is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the companion does not belong to the user.</response>
        /// <response code="404">If the companion or a referenced country/city is not found.</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(CompanionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanionResponse>> Update(
            int id,
            [FromForm] CompanionUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            return Ok(await _companionService.UpdateAsync(id, userId.Value, request, cancellationToken));
        }

        /// <summary>Deletes a companion belonging to the authenticated tourist.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if deletion was successful.</returns>
        /// <response code="204">The companion was deleted successfully.</response>
        /// <response code="400">If the ID is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the companion does not belong to the user.</response>
        /// <response code="404">If the companion is not found.</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid token.", StatusCodes.Status401Unauthorized));

            try
            {
                var deleted = await _companionService.DeleteAsync(id, userId.Value, cancellationToken);
                if (!deleted)
                    return NotFound(CreateProblemDetails("Companion not found", $"Companion with ID {id} not found", StatusCodes.Status404NotFound));
                return NoContent();
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
                _logger.LogError(ex, "Error deleting companion {CompanionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        #region Helpers

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
