using Application.DTOs.Favorite.Request;
using Application.DTOs.Favorite.Response;
using Application.DTOs.Pagination;
using Application.Interfaces.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireCompletedProfile")]
    [Authorize(Roles = "Tourist")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Adds a tour company to the authenticated user's favorites
        /// </summary>
        /// <param name="request">Favorite add request containing the company ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created favorite</returns>
        /// <response code="201">Returns the newly created favorite</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have a completed profile</response>
        /// <response code="404">If the referenced company is not found</response>
        /// <response code="409">If the company is already in the user's favorites</response>
        /// <response code="422">If the user attempts to favorite their own company</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add([FromBody] FavoriteAddRequest request, CancellationToken cancellationToken = default)
        {
            var success = await _favoriteService.AddAsync(request.CompanyId, cancellationToken);
            if (success)
                return Ok(new { message = $"company {request.CompanyId} has been added to your favorites." });
            return BadRequest(CreateProblemDetails("Bad Request", "Failed to add company to favorites.", StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Retrieves all favorite tour companies for the authenticated user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of favorite tour companies</returns>
        /// <response code="200">Returns the list of favorites</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have a completed profile</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<FavoriteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponse<FavoriteResponse>>> GetAll(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return Ok(await _favoriteService.GetUserFavoritesAsync(page, pageSize, cancellationToken));
        }

        #region Helpers

        private ProblemDetails CreateProblemDetails(string title, string detail, int statusCode = StatusCodes.Status500InternalServerError)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = HttpContext?.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }

        #endregion
    }
}
