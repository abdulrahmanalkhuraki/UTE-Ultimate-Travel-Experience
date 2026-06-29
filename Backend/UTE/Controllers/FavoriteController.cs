using Application.DTOs.Favorite.Request;
using Application.DTOs.Favorite.Response;
using Application.Interfaces.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(FavoriteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FavoriteResponse>> Add([FromBody] FavoriteAddRequest request, CancellationToken cancellationToken = default)
        {
            var created = await _favoriteService.AddAsync(request.CompanyId, cancellationToken);
            return CreatedAtAction(nameof(GetAll), null, created);
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
        [ProducesResponseType(typeof(IReadOnlyList<FavoriteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<FavoriteResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            return Ok(await _favoriteService.GetUserFavoritesAsync(cancellationToken));
        }
    }
}
