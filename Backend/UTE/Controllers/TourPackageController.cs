using System.Net.Mime;
using System.Security.Claims;
using Application.DTOs.Pagination;
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

        public TourPackageController(ITourPackageService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetAllAsync(cancellationToken));
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
            return Ok(await _service.GetMineAsync(id,cancellationToken));
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

                return Ok(await _service.FilterAsync(countryId, cityId, minPrice, maxPrice, cancellationToken));

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
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetUnApproved(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.GetUnApprovedAsync(cancellationToken));
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
