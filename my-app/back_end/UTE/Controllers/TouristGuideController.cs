using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.Pagination;
using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;
using Application.Exceptions;
using Application.Interfaces.TouristGuide;
using Microsoft.AspNetCore.Authorization;
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

        public TouristGuideController(ITouristGuideService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("mine")]
        [ProducesResponseType(typeof(PaginatedResponse<TouristGuideResponseSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponse<TouristGuideResponseSummary>>> GetMine(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetMineAsync(page, pageSize, cancellationToken));
        }

        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(TouristGuideResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TouristGuideResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetAsync(id, cancellationToken));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TouristGuideResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TouristGuideResponse>> Create([FromForm] TouristGuideCreateRequest request, 
            CancellationToken cancellationToken = default)
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

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
            return Ok(await _service.UpdateAsync(id, request, cancellationToken));
        }

        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(CreateProblemDetails("Guide not found", $"Guide with ID {id} not found", StatusCodes.Status404NotFound));
            return NoContent();
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
