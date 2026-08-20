using Application.DTOs.Review.Request;
using Application.DTOs.Review.Response;
using Application.Interfaces.Review;
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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Creates a new Review
        /// </summary>
        /// <param name="request">Review creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created Review</returns>
        /// <response code="201">Returns the newly created Review</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="404">If a referenced entity TourPackage is not found</response>
        /// <response code="422">If the user doesn't have any Completed booking in referenced Tour Package</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReviewResponse>> Create([FromBody] ReviewCreateRequest request, CancellationToken cancellationToken = default)
        {
            var created = await _reviewService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Retrieves a list of reviews, optionally filtered by user ID or tour package ID
        /// </summary>
        /// <param name="userId">Optional user ID to filter reviews</param>
        /// <param name="tourPackageId">Optional tour package ID to filter reviews</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of reviews matching the specified filters</returns>
        /// <response code="200">Returns the list of reviews</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<ReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> Get([FromQuery] int? userId, [FromQuery] int? tourPackageId, CancellationToken cancellationToken = default)
        {
            return Ok(await _reviewService.GetAsync(userId, tourPackageId, cancellationToken));
        }
    }
}
