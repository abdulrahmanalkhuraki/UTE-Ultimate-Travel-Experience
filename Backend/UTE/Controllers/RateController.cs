using Application.DTOs.Rate.Request;
using Application.DTOs.Rate.Response;
using Application.Interfaces.Booking;
using Application.Interfaces.Rate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;
        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }

        /// <summary>
        /// Creates a new Rating
        /// </summary>
        /// <param name="request">Rating creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created Rating</returns>
        /// <response code="201">Returns the newly created Rating</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="404">If a referenced entity TourPackage is not found</response>
        /// <response code="422">If the user doesn't have any Completed booking in referenced Tour Package</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(RateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RateResponse>> Create([FromBody] RateCreateRequest request,CancellationToken cancellationToken = default)
        {
            var created = await _rateService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get),new { id = created.Id },
                created);
        }


        /// <summary>
        /// Retrieves a list of ratings, optionally filtered by user ID or tour package ID
        /// </summary>
        /// <param name="userId">Optional user ID to filter ratings</param>
        /// <param name="packageId">Optional tour package ID to filter ratings</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of ratings matching the specified filters</returns>
        /// <response code="200">Returns the list of ratings</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<RateResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<RateResponse>>> Get([FromQuery] int? userId, [FromQuery] int? packageId,CancellationToken cancellationToken = default)
        {
            return Ok(await _rateService.GetAsync(userId, packageId, cancellationToken));
        }
    }
}
